const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT
  ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
  : env.ASPNETCORE_URLS
    ? env.ASPNETCORE_URLS.split(';')[0]
    : 'http://localhost:5161';

const PROXY_CONFIG = [
  {
    context: ["/weatherforecast"],
    target: target,
    secure: false,
    changeOrigin: true,
  },
  {
    context: ["/api"],
    target: "http://localhost:5161",
    secure: false,
    changeOrigin: true,
  },
];

module.exports = PROXY_CONFIG;
