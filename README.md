# NugetServer

Very simple and minimal v3 nuget server. Ideal for hosting internal nuget packages.

## Installation
The easiest way to get NugetServer up and running is to use the docker image. Or you can download the compiled version and run it behind IIS, Nginx etc...

### Docker (Linux image)
`docker run --publish 5555:8080 -v /nugetserver:/app/data --env NUG_URL=http://localhost:5555 --name nugetserver -d rebinf/nugetserver:latest`

### Settings/Environment Variables
You can set the following environment variables to configure the server:

`NUG_URL`: Base URL for the NugetServer.

`REQ_KEY`: Whether to require an API key for publishing or deleting packages.

`API_KEY`: API key for the NugetServer.

`DEL_ALL`: Whether to delete previous versions of the uploaded package on publish.

`DEL_MIN`: Whether to delete previous minor versions of the uploaded package on publish.

`DEL_PRE`: Whether to delete previous preview versions of the uploaded package on publish.

### Use with docker
`docker run --publish 5555:8080 --env NUG_URL=http://localhost:5555 --env API_KEY=1234567890 --name nugetserver -d rebinf/nugetserver-win:latest`

### appsettings.json
You can set these variables in the `appsettings.json` file.

## Usage
### Setup
Add your nuget server url to to your nuget.config or Visual Studio. Url format will be:\
`{YOUR_SERVER_URL}/v3/index.json`.

### Push packages
Push packages to your server using:\
`dotnet nuget push {PATH_TO_YOUR_NUPKG} --api-key 123456790 --source "{YOUR_SERVER_URL}"`

## Contributing
Any contributions are appreciated, You're more than welcome to open a Pull Request.
