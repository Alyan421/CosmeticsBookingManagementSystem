export const environment = {
  production: true,
  get apiUrl() {
    return (window as any).config?.apiUrl || 'https://your-fallback-url.azurewebsites.net/api';
  }
};
