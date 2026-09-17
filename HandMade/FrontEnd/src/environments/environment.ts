// Development: empty base so services emit relative /api/... paths and the
// dev-server proxy (proxy.conf.json) forwards them to the local API.
export const environment = {
  production: false,
  apiBaseUrl: ''
};
