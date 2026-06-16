# TODO List App

A small full-stack TODO list built with an Angular frontend and a .NET 9 Minimal API backend. State is held in memory on the server.

## Stack

- Frontend: Angular 19 (standalone components, signals, HttpClient)
- Backend: .NET 9 Minimal API (vertical-slice endpoints, in-memory singleton store)
- Tests: xUnit + `WebApplicationFactory` on the backend, one Karma/Jasmine spec on the frontend

## Prerequisites

Docker is the only thing required to run the app end-to-end. The Docker setup builds both services from source, so the reviewer does not need .NET or Node installed.

If you prefer to run the pieces directly, you need:

- .NET 9 SDK
- Node 20+

## Run with Docker

From the repository root:

```
docker compose up --build
```

Then open [http://localhost:8090](http://localhost:8090).

The Angular app is served by nginx on port 80 inside the container (mapped to 8090 on the host). The browser hits a single origin, and nginx proxies `/api/*` to the API container (exposed on host port 5050 if you want to call the API directly), so there is no CORS configuration to worry about in this mode.

To stop the stack: `docker compose down`.

## Run locally (without Docker)

Two terminals.

API:

```
cd TodoApp.Api
dotnet run
```

The API listens on the URLs configured in `Properties/launchSettings.json` (defaults to `https://localhost:5001` and `http://localhost:5000`). CORS is configured to allow `http://localhost:4200`.

Frontend:

```
cd TodoApp.Client
npm install
npm start
```

Then open [http://localhost:4200](http://localhost:4200).

## Tests

Backend:

```
cd TodoApp.Api.Tests
dotnet test
```

Frontend (optional, requires a Chrome browser installed locally):

```
cd TodoApp.Client
npm test -- --watch=false --browsers=ChromeHeadless
```

## API


| Method | Route             | Body                      | Response                                    |
| ------ | ----------------- | ------------------------- | ------------------------------------------- |
| GET    | `/api/todos`      | -                         | `200` array of items                        |
| POST   | `/api/todos`      | `{ "title": "Buy milk" }` | `201` created item; `400` if title is empty |
| DELETE | `/api/todos/{id}` | -                         | `204` on success; `404` if not found        |
| PATCH  | `/api/todos/{id}` | `{ "isDone": true }`      | `200` updated item; `404` if not found      |

## Demo

See `demo-assets/todo-app-demo.mp4` for a short screen recording of the app running end-to-end.

## Notes

- State is in-memory. The list resets whenever the API process restarts. In Docker that means every `docker compose up` of a fresh container starts with an empty list.
- The frontend uses a relative `/api/todos` URL. In Docker that goes through the nginx proxy. In local dev, the Angular dev server talks to the API directly and CORS handles the cross-origin request.
- The frontend has no edit-in-place flow; items are added, toggled, and deleted.

