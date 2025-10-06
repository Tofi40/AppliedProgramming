# RoomieMatch Frontend

This Angular 17 single-page application provides a lightweight interface on top of the RoomieMatch ASP.NET backend. It offers authentication, room browsing, booking request management, and profile editing flows backed by the API located at `http://localhost:5000/api`.

## Features
- Email + password registration and login for seekers and owners.
- Landing page with featured rooms and quick navigation.
- Filterable rooms catalog with compatibility scores for seekers.
- Room detail view with booking request submission for seekers.
- Dashboard summarising rooms and latest requests.
- Requests management surface with owner status updates.
- Profile editor for managing personal information and interests.

## Local development

> **Note:** Package installation requires Node.js 18+ and access to the public npm registry.

```bash
cd frontend
npm install
npm start
```

The dev server runs on `http://localhost:4200` and proxies API calls directly to the backend using absolute URLs. Update `src/environments/environment*.ts` if you host the API elsewhere.

## Testing

Run unit tests with:

```bash
npm test
```

ESLint can be executed via:

```bash
npm run lint
```

## Environment variables

Adjust the default API base URL by editing the environment files. During local development the application targets `http://localhost:5000/api` to match the default backend port.

## Folder structure

```
frontend/
├── src/
│   ├── app/
│   │   ├── core/              # Shared services, guards and models
│   │   ├── features/          # Routed pages (auth, rooms, dashboard, etc.)
│   │   ├── app.component.*    # Root shell and layout
│   │   └── app.routes.ts      # Route definitions
│   ├── environments/          # API configuration per build profile
│   ├── main.ts                # Application bootstrap
│   └── styles.scss            # Global theme styling
├── angular.json               # Angular CLI project configuration
└── package.json               # Dependencies and npm scripts
```

