import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  constructor() { }

  success(message: string): void {
    this.showNotification(message, 'success');
    console.log('Success:', message);
  }

  error(message: string): void {
    this.showNotification(message, 'error');
    console.error('Error:', message);
  }

  info(message: string): void {
    this.showNotification(message, 'info');
    console.info('Info:', message);
  }

  warning(message: string): void {
    this.showNotification(message, 'warning');
    console.warn('Warning:', message);
  }

  private showNotification(message: string, type: string): void {
    // Create notification element
    const notification = document.createElement('div');
    notification.className = `notification ${type}`;
    notification.innerHTML = `
      <div class="notification-content">${message}</div>
      <button class="notification-close">&times;</button>
    `;

    // Style the notification
    notification.style.position = 'fixed';
    notification.style.top = '20px';
    notification.style.right = '20px';
    notification.style.zIndex = '9999';
    notification.style.minWidth = '250px';
    notification.style.padding = '15px';
    notification.style.borderRadius = '4px';
    notification.style.display = 'flex';
    notification.style.justifyContent = 'space-between';
    notification.style.alignItems = 'center';
    notification.style.boxShadow = '0 4px 8px rgba(0,0,0,0.1)';

    // Set color based on type
    if (type === 'success') {
      notification.style.backgroundColor = '#4CAF50';
      notification.style.color = 'white';
    } else if (type === 'error') {
      notification.style.backgroundColor = '#F44336';
      notification.style.color = 'white';
    } else if (type === 'info') {
      notification.style.backgroundColor = '#2196F3';
      notification.style.color = 'white';
    } else if (type === 'warning') {
      notification.style.backgroundColor = '#FF9800';
      notification.style.color = 'white';
    }

    // Style close button
    const closeBtn = notification.querySelector('.notification-close') as HTMLElement;
    if (closeBtn) {
      closeBtn.style.background = 'transparent';
      closeBtn.style.border = 'none';
      closeBtn.style.color = 'white';
      closeBtn.style.fontSize = '20px';
      closeBtn.style.cursor = 'pointer';
      closeBtn.style.marginLeft = '10px';

      // Add click event to close button
      closeBtn.addEventListener('click', () => {
        document.body.removeChild(notification);
      });
    }

    // Add to DOM
    document.body.appendChild(notification);

    // Auto-remove after 3 seconds
    setTimeout(() => {
      if (document.body.contains(notification)) {
        document.body.removeChild(notification);
      }
    }, 3000);
  }
}
