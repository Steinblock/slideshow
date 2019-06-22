FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
#EXPOSE 80
#EXPOSE 443
EXPOSE 5000

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["src/slideshow/slideshow.csproj", "slideshow/"]
COPY ["src/slideshow.db/slideshow.db.csproj", "slideshow.db/"]
COPY ["src/slideshow.data/slideshow.data.csproj", "slideshow.data/"]
COPY ["src/slideshow.core/slideshow.core.csproj", "slideshow.core/"]
COPY ["src/slideshow.db.sqlite/slideshow.db.sqlite.csproj", "slideshow.db.sqlite/"]
RUN dotnet restore "slideshow/slideshow.csproj"
COPY src .
WORKDIR /src/slideshow
RUN dotnet build "slideshow.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "slideshow.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
COPY src/slideshow/slideshow.db .
# COPY migrate.sh .

# https://stackoverflow.com/questions/48669548/why-does-aspnet-core-start-on-port-80-from-within-docker/48669703
# https://docs.microsoft.com/de-de/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-2.2#endpoint-configuration
ENV ASPNETCORE_URLS http://0.0.0.0:5000

ENTRYPOINT ["dotnet", "slideshow.dll"]