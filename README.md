# FamilyBudget

To install Family Budget app you will need to have Docker installed

After this you have to install certificate on your local machine.

On Windows run following in command line:
```
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p password
dotnet dev-certs https --trust
```

(please don't change certificate name in command because it is included in docker-compose file)

On other operating system follow [this article](https://learn.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-7.0)

Use docker-compose commands to get images and run app
```
docker-compose pull

docker-compose up -d
```

You have predefined user with login: "admin" and password: "password" (for easier use)

Rest of created users password is "test".

You can modify default app behaviors by changing docker-compose file


## !!! Important
### Sql Server database don't have mounted volume so naturally after stopping image all data will be gone