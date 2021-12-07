# backend

The backend for the awesome fair-teams-ai.

## Manual build & deploy

You'll need the [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) as it's an `ASP.NET Core` app, serving a REST Api for the frontend.

To restore the dependencies, build the app and run it locally just type:

``` bash
cd backend
dotnet run
```

The OpenAPI REST API description should then be reachable in a browser on <http://localhost/swagger/index.html>

## Docker

If you prefer Docker, you can either build the image yourself:

```bash
cd backend
docker build -t fair-teams-ai .
docker run -P fair-teams-ai
```

This will also build the .NET application - just inside the container.
Alternatively, a pre-build, multi-platform (`linux/amd64` and `linux/arm64`) image is also up on [dockerhub](https://hub.docker.com/r/floriang89/fair-teams-ai).

```bash
docker run -P floriang89/fair-teams-ai:latest
```

Both options should make OpenAPI reachable under the same URL as above (<http://localhost/swagger/index.html>).

## Continous integration

GitHub Actions run our automated tests for the frontend and backend. The backend tests, especially the demo analyzer tests, require a large
amount of test data. That data is not checked into the repository, but stored in an Azure Blob Storage and downloaded at runtime.

For this to work, the environment variable `AZURE_STORAGE_CONNECTION_STRING` has to be set to the connection string of the blob container.
