# backend

The backend for the awesome fair-teams-ai.

## Manual build & deploy

You'll need the [.NET 5.0 SDK](https://dotnet.microsoft.com/download/dotnet/5.0) as it's an `ASP.NET Core` app, serving a REST Api for the frontend.

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

## Continuous deployment

To enable continuous deployment of newly created and pushed docker image we use [docker-image-puller](https://github.com/tuxity/docker-image-puller). You need to run this container on the machine you're running the backend on:

```bash
docker run -d \
  --name docker-image-puller \
  --env TOKEN=TOKYMCTOKENFACE \
  -p 8080:8080 \
  -v /var/run/docker.sock:/var/run/docker.sock \
  tuxity/docker-image-puller
```

This will listen on port `8080` for a `POST` request in this format:

```bash
http://ipofyourserver/images/pull?token=TOKYMCTOKENFACE&restart_containers=true&image=floriang89/fair-teams-ai:latest
```

We send a request in that format in our GitHub Actions CI/CD pipeline after successfully building and pushing a new docker image of the backend.
