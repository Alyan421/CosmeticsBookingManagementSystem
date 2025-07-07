import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
@Component({
  selector: 'app-not-found',
  template: `<h1>404 - Page Not Found</h1>`,
  styleUrls: ['./not-found.component.css'],
  standalone: true,
  imports: [CommonModule],
})
export class NotFoundComponent { }
