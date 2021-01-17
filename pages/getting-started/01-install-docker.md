# Install Docker

General information on how to install Docker on the available operating systems can be found here:

[docker.com - Get Docker](https://docs.docker.com/get-docker/)

## Windows Server

Take note that when looking for "Install Docker on Windows", 9 out of 10 results point to Docker Desktop.
This is nice for development, but you don't want to do this on Windows Server 2016+ installations.

The short version (all powershell):

```powershell
# Enable the "Containers" feature
Install-WindowsFeature Containers -Restart

# Install the Docker package provider
Install-Module -Name DockerMsftProvider -Repository PSGallery -Force

# Install Docker
Install-Package -Name docker -ProviderName DockerMsftProvider

# Start the Docker service
Start-Service Docker
```

**Test it out.**
```powershell
docker run hello-world
```

For a longer explanation, see this post from [Elton Stoneman](https://blog.sixeyed.com/):

[Getting Started with Docker on Windows Server 2019](https://blog.sixeyed.com/getting-started-with-docker-on-windows-server-2019/)

## Linux

The quickest way to get Docker working on Linux, is probably using the convenience script.
This script automatically detects the distribution in use and pulls in all the required bits.

This is not necessarily the best way though, since it might not correctly detect the running distro,
and it also requires sudo, after which it executes a pile of scripts that directly come from the internet.

Having given you those warning, [here you go](https://docs.docker.com/engine/install/ubuntu/#install-using-the-convenience-script):

```bash
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
```
