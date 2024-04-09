# Intro

This repo is a collection of notes on creating and using containers with Docker Compose and is referenced by the following two blog articles:

- [Part I: Microsoft SQL Server with Docker Compose](https://m7y.me/post/2024-04-05-sql-server-with-docker-compose/)
- [Part II: Initialising SQL Server with Docker Compose](https://m7y.me/post/2024-04-06-sql-server-init-docker-compose/)

# ./mssql

Examples of `mssql` containers. [ReadMe](./mssql/README.md).

# ./other-dbs

Examples of some other popular db containers. [ReadMe](./other-dbs/README.md).

- mysql
- postgres
- redis

# ./sonarqube

Example of `sonarqube` setup and scanning code via docker. [ReadMe](./sonarqube//README.md).

# Cleanup and Teardown Examples

Managing the lifecycle of your Docker containers is an important aspect of working with containers.

## `down`

To stop and remove the SQL server container created with `docker-compose`, use the following command from the directory containing your `docker-compose-mssql.yml` file:

```bash
docker-compose -f docker-compose-mssql.yml down
```

This command stops and removes the containers, networks, and the default network, but not the volumes, preserving your data. If you wish to completely remove the data and start afresh, you can manually delete the folders specified in the volumes section.

## `stop` & `start`

`docker-compose` also provides the option to only stop the services:

```bash
docker-compose -f docker-compose-mssql.yml stop
```

This command stops the running containers without removing them, allowing you to start them again later with `start`.

```bash
docker-compose -f docker-compose-mssql.yml start
```

## `--force-recreate` option

The `--force-recreate` flag is a useful option for ensuring that containers are freshly created to reflect the latest configurations, especially when changes are made that don't affect the image directly. It's used with the `docker-compose up` command, forcing `docker-compose` to ignore and remove existing containers and create new ones from the updated configuration or environment variables. This option is particularly useful when troubleshooting unexpected behavior by starting with a clean state.
