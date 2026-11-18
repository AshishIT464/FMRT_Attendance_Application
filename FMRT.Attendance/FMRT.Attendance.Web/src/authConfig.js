export const msalConfig = {
  auth: {
    clientId: "15fdf7ae-8f6a-4c26-a533-70e15758a1e8",

    authority:
      "https://login.microsoftonline.com/09922a10-e62b-4968-a3e9-1c2c6cf768db",

    redirectUri: "https://192.168.0.253"
  },

  cache: {
    cacheLocation: "localStorage",
    storeAuthStateInCookie: false
  }
};

export const loginRequest = {
  scopes: [
    "openid",
    "profile",
    "email",
    "User.Read"
  ]
};