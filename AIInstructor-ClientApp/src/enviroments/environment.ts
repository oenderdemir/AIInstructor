export const environment = {
    production: false,
    apiUrl: (window as any).__env?.apiUrl || 'http://localhost:7215/ui'
  };