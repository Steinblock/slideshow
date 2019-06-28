# Slideshow

Example Project for .NET Usergroup Rhein/Ruhr

Jürgen Steinblock - 2019

## Demo / Usage

* clone repository and run `dotnet run --project src/slideshow/slideshow.csproj`
* enable slide editing / creation: login as user admin / somepassword (only in development-mode)
  * for production set Environment Variables (see, UserManager.CreateSalt(), UserManager.CreateHash(...))
    * `USERMANAGER_SALT=...`
	* `USERMANAGER_ADMIN_HASH=...`
* run tests with `dotnet test /p:CollectCoverage=true`

or (to just see a live version of the slideshow) ...

* [![Open in Gitpod](https://gitpod.io/button/open-in-gitpod.svg)](https://gitpod.io#snapshot/6b6501b7-f9dd-40b1-b100-e0fdc088cf66) and wait for build to complete. Application will open on the side.
* gitpod requires a github account.
* gitpod is a [theia](https://www.theia-ide.org/) based web IDE, that brings the VSCode experience to the cloud. See [gitpod features](https://www.gitpod.io/features)
* see `.gitpod.yml` for building and testing the app

## Features

### ASP.NET Core MVC WebApplication

* Controller and Models in project `slideshow.web`
* Views and Assets (wwwroot) in project `slideshow`
* Hosting as as standalone app, (windows) service, with IIS or as a docker container

### TopShelf

* the application uses TopShelf to run as a windows service

```bash
# publish exe
dotnet publish -c Release -r win10-x64
# cd to publish dir
cd src\slideshow\bin\Release\netcoreapp2.1\win10-x64\publish\
# show topsehlf help
slideshow.exe --help
# install service
slideshow.exe install
# start service
slideshow.exe start
```

### Dependency Injection with Ninject

* extending the application is easy:
  * interfaces are defined in project `slideshow.core`
  * create new project and reference `slideshow.core`
  * load the project in `Program.cs`: `kernel.Load("slideshow.extension.dll");`
  * add a reference to ninject in project `slideshow.extension.csproj`: `<PackageReference Include="Ninject" Version="3.3.4" />`
  * create a new NinjectModule in project `slideshow.extensions`:  `public class SampleModule : NinjectModule`
  * add at least one binding
    ```csharp
    public class SampleModule : NinjectModule
    {
        public override void Load()
        {
            this.Bind<IService>().To<SampleService>()
                .InSingletonScope()
                .WithConstructorArgument("arg", value);
        }
    }
    ```

### Entity Framework Core Datenbank

* Supports SQLite and Postgresql (default SQLite)
* For Postgres, set environment variable `DATABASE_URL`: `DATABASE_URL=postgres://postgres:postgres@localhost:5432/slideshow`
* Models are defined in project `slideshow.data`. After a model change add a migration:
   ```powershell
   # postgres
   $env:DATABASE_URL="postgres://postgres:postgres@localhost:5432/slideshow"
   dotnet ef migrations add MigrationName --project src/slideshow.db.postgres --startup-project src/slideshow --context PostgresSlideshowContext
   dotnet ef database update --project src/slideshow.db.postgres --startup-project src/slideshow --context PostgresSlideshowContext

   # sqlite
   dotnet ef migrations add InitialCreate --project src/slideshow.db.sqlite --startup-project src/slideshow --context SqliteSlideshowContext
   dotnet ef database update --project src/slideshow.db.sqlite --startup-project src/slideshow --context SqliteSlideshowContext
   ```
* run migration in production app: `dotnet slideshow.dll --migrate`

### Unit-Tests

* Projekt `slideshow.web.tests`
* Test isolation with Moq

### Quartz Scheduler

* For a sample, see `slideshow.scheduler.jobs` Klasse `jobs/SampleJob.cs`

### Docker Support

* run and debug the application in an isolated container
* build and publish the app to a docker registry

### GitLab AutoDevOps

* Example build script with `build / test / deploy (Docker-Image)` pipeline in `.gitlab-ci.yml.bak`
* Modified AutoDevOps Template for dotnet in `.gitlab-ci.yml` with SAST / Dependency Scanning support
* If you push the project to a kubernetes enabled gitlab repository the project is automagically deployed to the cluster
* Custom liveness probe with service awarness: If the scheduler is disabled on the status page the http status code will be set to `422 (Unprocessable Entity)` and Kubernetes will restart the pod.

```yml
  LIVENESSPROBE_PATH: /status
  HELM_UPGRADE_EXTRA_ARGS: '--set livenessProbe.path="$LIVENESSPROBE_PATH"'
```

### Sentry.io Support

* Set environment variable `sentry__Dsn` to Enable Sentry support

### Unleash Support

* Set environment variable `UNLEASH_API_URL` and `UNLEASH_INSTANCE_ID` to enable Unleash support

### Notes

* To expose environment variables to the running kubernetes environment you have to prepending the variable key with `K8S_SECRET_` (i.e. `K8S_SECRET_sentry__Dsn`)
