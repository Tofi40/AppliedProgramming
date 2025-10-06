import { CurrencyPipe, NgClass, NgFor, NgIf } from '@angular/common';
import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Observable, catchError, forkJoin, map, of, switchMap, take } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';
import { MatchesService } from '../../core/services/matches.service';
import { RequestsService } from '../../core/services/requests.service';
import { RoomsService } from '../../core/services/rooms.service';
import { Match } from '../../core/models/match.model';
import { RoomSummary } from '../../core/models/room.model';

interface SwipeCardModel {
  match: Match;
  mode: 'seeker' | 'owner';
  title: string;
  subtitle?: string;
  description?: string;
  chips: string[];
  matchPercent: number;
  rationale: string[];
  pricePerMonthCents?: number;
  billsIncluded?: boolean;
  imageSeed: string;
  imageLabel: string;
  room?: RoomSummary | null;
}

interface RoomLookupResult {
  matches: Match[];
  rooms: Map<string, RoomSummary>;
}

@Component({
  standalone: true,
  selector: 'app-rooms-list',
  templateUrl: './rooms-list.component.html',
  styleUrl: './rooms-list.component.scss',
  imports: [NgIf, NgFor, NgClass, CurrencyPipe]
})
export class RoomsListComponent {
  private readonly authService = inject(AuthService);
  private readonly matchesService = inject(MatchesService);
  private readonly requestsService = inject(RequestsService);
  private readonly roomsService = inject(RoomsService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  private gradientCache = new Map<string, string>();
  private pointerId: number | null = null;
  private dragOrigin: { x: number; y: number } | null = null;
  private finishTimer: number | null = null;
  private feedbackTimer: number | null = null;
  private currentUserId: string | null = null;

  readonly mode = signal<'seeker' | 'owner' | 'guest'>('guest');
  readonly deck = signal<SwipeCardModel[]>([]);
  readonly loading = signal<boolean>(false);
  readonly cardPosition = signal({ x: 0, y: 0, rotation: 0 });
  readonly isDragging = signal(false);
  readonly cardTransition = signal(false);
  readonly animateDirection = signal<'left' | 'right' | null>(null);
  readonly feedback = signal<string | null>(null);

  readonly currentCard = computed(() => this.deck()[0] ?? null);
  readonly nextCard = computed(() => this.deck()[1] ?? null);
  readonly cardTransform = computed(() => {
    const { x, y, rotation } = this.cardPosition();
    return `translate3d(${x}px, ${y}px, 0) rotate(${rotation}deg)`;
  });
  readonly previewDecision = computed<'left' | 'right' | null>(() => {
    if (this.animateDirection()) {
      return this.animateDirection();
    }
    if (!this.isDragging()) {
      return null;
    }
    const x = this.cardPosition().x;
    if (x > 70) {
      return 'right';
    }
    if (x < -70) {
      return 'left';
    }
    return null;
  });
  readonly cardClasses = computed(() => ({
    'swipe-card--transition': this.cardTransition(),
    'swipe-card--dragging': this.isDragging(),
    'swipe-card--like': this.previewDecision() === 'right',
    'swipe-card--nope': this.previewDecision() === 'left'
  }));
  readonly headerTitle = computed(() =>
    this.mode() === 'owner' ? 'Review your top seeker matches' : 'Discover your next room'
  );
  readonly headerSubtitle = computed(() =>
    this.mode() === 'owner'
      ? 'Swipe through curated seekers and keep the best fits for your listings.'
      : 'Swipe right on rooms you love to send a request. Swipe left to keep browsing.'
  );

  constructor() {
    this.authService.session$
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((session) => {
        if (!session) {
          this.mode.set('guest');
          this.currentUserId = null;
          this.deck.set([]);
          return;
        }
        const role = session.role === 'Owner' ? 'owner' : 'seeker';
        const userChanged = this.currentUserId !== session.id || this.mode() !== role;
        this.mode.set(role);
        this.currentUserId = session.id;
        if (userChanged) {
          this.reload();
        }
      });

    this.destroyRef.onDestroy(() => {
      if (this.finishTimer) {
        window.clearTimeout(this.finishTimer);
      }
      if (this.feedbackTimer) {
        window.clearTimeout(this.feedbackTimer);
      }
    });
  }

  reload(): void {
    const role = this.mode();
    const userId = this.currentUserId;
    if (!userId || role === 'guest') {
      this.deck.set([]);
      return;
    }
    if (role === 'seeker') {
      this.loadSeekerDeck(userId);
    } else {
      this.loadOwnerDeck(userId);
    }
  }

  gradientFor(seed: string | undefined): string {
    if (!seed) {
      return 'linear-gradient(135deg, #a855f7 0%, #2563eb 100%)';
    }
    const key = seed.toLowerCase();
    if (this.gradientCache.has(key)) {
      return this.gradientCache.get(key)!;
    }
    const colors = this.computeGradient(key);
    const gradient = `linear-gradient(135deg, ${colors[0]} 0%, ${colors[1]} 100%)`;
    this.gradientCache.set(key, gradient);
    return gradient;
  }

  startDrag(event: PointerEvent): void {
    if (!this.currentCard() || this.animateDirection()) {
      return;
    }
    this.pointerId = event.pointerId;
    this.dragOrigin = { x: event.clientX, y: event.clientY };
    this.isDragging.set(true);
    this.cardTransition.set(false);
    (event.currentTarget as HTMLElement).setPointerCapture(event.pointerId);
  }

  drag(event: PointerEvent): void {
    if (!this.isDragging() || this.pointerId !== event.pointerId || !this.dragOrigin) {
      return;
    }
    const dx = event.clientX - this.dragOrigin.x;
    const dy = event.clientY - this.dragOrigin.y;
    this.cardPosition.set({ x: dx, y: dy, rotation: dx / 14 });
  }

  endDrag(event: PointerEvent): void {
    if (this.pointerId !== null && event.pointerId !== this.pointerId) {
      return;
    }
    if (!this.isDragging()) {
      return;
    }
    const x = this.cardPosition().x;
    this.isDragging.set(false);
    if (Math.abs(x) > 90) {
      this.commitDecision(x > 0 ? 'right' : 'left');
    } else {
      this.resetCardPosition();
    }
    if ((event.currentTarget as HTMLElement).hasPointerCapture(event.pointerId)) {
      (event.currentTarget as HTMLElement).releasePointerCapture(event.pointerId);
    }
    this.pointerId = null;
    this.dragOrigin = null;
  }

  skip(): void {
    if (!this.currentCard()) {
      return;
    }
    this.commitDecision('left');
  }

  like(): void {
    if (!this.currentCard()) {
      return;
    }
    this.commitDecision('right');
  }

  openDetails(): void {
    const card = this.currentCard();
    if (!card) {
      return;
    }
    if (card.mode === 'seeker' && card.match.roomId) {
      this.router.navigate(['/rooms', card.match.roomId]);
    } else {
      this.router.navigate(['/requests']);
    }
  }

  private loadSeekerDeck(seekerId: string): void {
    this.loading.set(true);
    this.matchesService
      .forSeeker(seekerId)
      .pipe(
        switchMap((matches) => this.attachRooms(matches, seekerId)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: ({ matches, rooms }) => {
          const sorted = [...matches].sort(
            (a, b) => (b.compatibilityScore ?? 0) - (a.compatibilityScore ?? 0)
          );
          const cards = sorted.map((match) => this.createSeekerCard(match, rooms.get(match.roomId) ?? null));
          this.deck.set(cards);
          this.loading.set(false);
        },
        error: () => {
          this.deck.set([]);
          this.loading.set(false);
          this.showFeedback('We could not load matches right now. Please try again.');
        }
      });
  }

  private loadOwnerDeck(ownerId: string): void {
    this.loading.set(true);
    this.matchesService
      .forOwner(ownerId)
      .pipe(
        switchMap((matches) => this.attachRooms(matches)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: ({ matches, rooms }) => {
          const sorted = [...matches].sort(
            (a, b) => (b.compatibilityScore ?? 0) - (a.compatibilityScore ?? 0)
          );
          const cards = sorted.map((match) => this.createOwnerCard(match, rooms.get(match.roomId) ?? null));
          this.deck.set(cards);
          this.loading.set(false);
        },
        error: () => {
          this.deck.set([]);
          this.loading.set(false);
          this.showFeedback("We couldn't load your seeker matches just yet.");
        }
      });
  }

  private attachRooms(matches: Match[], seekerId?: string): Observable<RoomLookupResult> {
    if (!matches.length) {
      return of({ matches, rooms: new Map<string, RoomSummary>() });
    }
    const roomIds = Array.from(new Set(matches.map((match) => match.roomId)));
    const roomRequests = roomIds.map((roomId) =>
      this.roomsService.getRoom(roomId, seekerId).pipe(catchError(() => of(null)))
    );
    return forkJoin(roomRequests).pipe(
      map((rooms) => {
        const lookup = new Map<string, RoomSummary>();
        rooms.filter((room): room is RoomSummary => room !== null).forEach((room) => lookup.set(room.id, room));
        return { matches, rooms: lookup } satisfies RoomLookupResult;
      })
    );
  }

  private createSeekerCard(match: Match, room: RoomSummary | null): SwipeCardModel {
    const details = room ?? null;
    const chips = new Set<string>();
    if (details?.billsIncluded) {
      chips.add('Bills included');
    }
    this.parseRationale(match.rationale)
      .slice(0, 2)
      .forEach((item) => chips.add(item));

    const title = details?.title ?? 'Featured match';
    const subtitle = details ? `${details.city}, ${details.country}` : 'Listing details will appear once loaded.';
    const description = details?.description ?? 'We found a promising room for you. Swipe right to bookmark it or refresh to load more details.';

    return {
      match,
      mode: 'seeker',
      title,
      subtitle,
      description,
      chips: Array.from(chips).slice(0, 4),
      matchPercent: this.asPercent(match.compatibilityScore),
      rationale: this.parseRationale(match.rationale),
      pricePerMonthCents: details?.pricePerMonthCents,
      billsIncluded: details?.billsIncluded,
      imageSeed: details?.title ?? match.roomId,
      imageLabel: (details?.title ?? 'Room').charAt(0).toUpperCase(),
      room: details
    };
  }

  private createOwnerCard(match: Match, room: RoomSummary | null): SwipeCardModel {
    const seekerCode = match.seekerId.split('-')[0]?.toUpperCase() ?? match.seekerId.substring(0, 6).toUpperCase();
    const chips = this.parseRationale(match.rationale).slice(0, 3);
    if (room) {
      chips.unshift(`${room.city}, ${room.country}`);
    }
    return {
      match,
      mode: 'owner',
      title: `Seeker ${seekerCode}`,
      subtitle: room ? `Great fit for ${room.title}` : undefined,
      description:
        room
          ? `This seeker is trending for ${room.title}. Swipe right to flag them for outreach.`
          : 'Swipe right to save this seeker for later follow-up.',
      chips: Array.from(new Set(chips)).slice(0, 4),
      matchPercent: this.asPercent(match.compatibilityScore),
      rationale: this.parseRationale(match.rationale),
      imageSeed: match.seekerId,
      imageLabel: seekerCode.charAt(0) || '👤',
      room
    };
  }

  private commitDecision(direction: 'left' | 'right'): void {
    if (!this.currentCard() || this.animateDirection()) {
      return;
    }
    const exitX = direction === 'right' ? window.innerWidth : -window.innerWidth;
    this.animateDirection.set(direction);
    this.cardTransition.set(true);
    this.cardPosition.set({ x: exitX, y: this.cardPosition().y + 40, rotation: direction === 'right' ? 28 : -28 });
    const selectedCard = this.currentCard();
    this.finishTimer = window.setTimeout(() => {
      if (selectedCard) {
        this.processDecision(direction, selectedCard);
      }
      this.deck.update((cards) => cards.slice(1));
      this.cardTransition.set(false);
      this.animateDirection.set(null);
      this.cardPosition.set({ x: 0, y: 0, rotation: 0 });
      this.isDragging.set(false);
    }, 260);
  }

  private processDecision(direction: 'left' | 'right', card: SwipeCardModel): void {
    if (direction === 'left') {
      this.showFeedback(`You passed on ${card.title}.`);
      return;
    }

    if (card.mode === 'seeker') {
      this.requestsService
        .create({ roomId: card.match.roomId })
        .pipe(take(1))
        .subscribe({
          next: () => this.showFeedback(`Sent a booking request for ${card.title}.`),
          error: () => this.showFeedback(`Saved ${card.title}; send a request once you're back online.`)
        });
    } else {
      const roomTitle = card.room?.title;
      this.showFeedback(
        roomTitle ? `Saved this seeker for ${roomTitle}.` : 'Saved this seeker to review later.'
      );
    }
  }

  private resetCardPosition(): void {
    this.cardTransition.set(true);
    this.cardPosition.set({ x: 0, y: 0, rotation: 0 });
    window.setTimeout(() => this.cardTransition.set(false), 220);
  }

  private parseRationale(rationale?: string | null): string[] {
    if (!rationale) {
      return [];
    }
    return rationale
      .split(/\r?\n|\.|\!/)
      .map((line) => line.trim())
      .filter((line) => line.length > 0)
      .map((line) => line.replace(/^[-•\s]+/, ''));
  }

  private asPercent(score: number | null | undefined): number {
    if (score == null) {
      return 0;
    }
    const percent = Math.round(Math.max(0, Math.min(1, score)) * 100);
    return percent;
  }

  private computeGradient(seed: string): [string, string] {
    let hash = 0;
    for (let i = 0; i < seed.length; i++) {
      hash = seed.charCodeAt(i) + ((hash << 5) - hash);
    }
    const color = (offset: number) => {
      const value = (hash >> offset) & 0xff;
      return `hsl(${(value * 137) % 360}, 75%, 60%)`;
    };
    return [color(0), color(8)];
  }

  private showFeedback(message: string): void {
    this.feedback.set(message);
    if (this.feedbackTimer) {
      window.clearTimeout(this.feedbackTimer);
    }
    this.feedbackTimer = window.setTimeout(() => this.feedback.set(null), 3200);
  }
}
