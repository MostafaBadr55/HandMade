// Production: set this to the deployed API's origin (e.g. 'https://api.example.com')
// before shipping — there is no dev-server proxy in a production build.
// UseCors is enabled on the API (Program.cs) so a frontend served from a
// different origin still works.
export const environment = {
  production: true,
  apiBaseUrl: ''
};
