// This file is used to inject runtime environment variables into the app.
// In production you can replace this file at container start with values from the host
// Example replacement content:
// window.__env = { REACT_APP_API_URL: 'https://api.example.com' };

(function (window) {
  window.__env = window.__env || {};

  // Default values - can be overridden by replacing this file before the container serves static files
  window.__env.REACT_APP_API_URL = window.__env.REACT_APP_API_URL || 'http://localhost:5304';
  window.__env.REACT_APP_ENVIRONMENT = window.__env.REACT_APP_ENVIRONMENT || 'development';
})(this);