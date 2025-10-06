import { Component, computed, inject } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AsyncPipe, NgIf } from '@angular/common';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterModule, NgIf, AsyncPipe],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  private readonly auth = inject(AuthService);
  readonly user$ = this.auth.user$;
  readonly isAuthenticated$ = this.auth.isAuthenticated$;

  readonly role = computed(() => this.auth.currentUser()?.role ?? null);

  logout(): void {
    this.auth.logout();
  }
}
