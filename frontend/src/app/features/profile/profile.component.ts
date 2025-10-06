import { Component, inject, OnInit, signal } from '@angular/core';
import { AsyncPipe, NgFor, NgIf } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';
import { UsersService } from '../../core/services/users.service';
import { UserProfile } from '../../core/models/user.model';

@Component({
  standalone: true,
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss',
  imports: [ReactiveFormsModule, NgIf, NgFor, AsyncPipe]
})
export class ProfileComponent implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly usersService = inject(UsersService);
  private readonly fb = inject(FormBuilder);

  readonly profile = signal<UserProfile | null>(null);
  readonly saving = signal<boolean>(false);
  readonly message = signal<string>('');

  readonly form = this.fb.group({
    displayName: ['', [Validators.required]],
    city: [''],
    country: [''],
    bio: [''],
    interests: [''],
    habits: [''],
    personality: ['']
  });

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.authService.refreshProfile().subscribe((profile) => {
      if (!profile) {
        return;
      }
      this.profile.set(profile);
      this.form.patchValue({
        displayName: profile.displayName,
        city: profile.city,
        country: profile.country,
        bio: profile.bio ?? '',
        interests: profile.interests?.join(', ') ?? '',
        habits: profile.habits?.join(', ') ?? '',
        personality: profile.personality?.join(', ') ?? ''
      });
    });
  }

  save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    const value = this.form.value;
    const payload = {
      displayName: value.displayName ?? undefined,
      city: value.city ?? undefined,
      country: value.country ?? undefined,
      bio: value.bio ?? undefined,
      interests: value.interests ? value.interests.split(',').map((item) => item.trim()).filter(Boolean) : undefined,
      habits: value.habits ? value.habits.split(',').map((item) => item.trim()).filter(Boolean) : undefined,
      personality: value.personality ? value.personality.split(',').map((item) => item.trim()).filter(Boolean) : undefined
    };

    this.usersService.updateMe(payload).subscribe({
      next: (profile) => {
        this.profile.set(profile);
        this.message.set('Profile updated');
        this.saving.set(false);
      },
      error: () => {
        this.message.set('Could not update profile');
        this.saving.set(false);
      }
    });
  }
}
