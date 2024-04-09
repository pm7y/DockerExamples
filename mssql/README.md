# mssql

## SQL Server Container

Simpley create a SQL server container.

```bash
docker-compose -f docker-compose-mssql.yml up -d
```

## SQL Server Container (`command` initialisation)

Create a SQL server container and override `command` to execute an init SQL script via `sqlcmd`.

```bash
docker-compose -f docker-compose-mssql.yml -f docker-compose-mssql-init-cmd.yml up -d
```

## SQL Server Container (`mssql-tools` initialisation)

Create a SQL server container and an `mssql-tools` container to execute an init SQL script via `sqlcmd`.

```bash
docker-compose -f docker-compose-mssql.yml -f docker-compose-mssql-init-tools.yml up -d
```

## SQL Server Container (`bacpac` initialisation)

Create a SQL server container and a `dotnet/sdk` container to execute `SqlPackage` to restore a `bacpac`.

```bash
docker-compose -f docker-compose-mssql.yml -f docker-compose-mssql-init-bacpac.yml up -d
```

## mssql on `arm` [optional]

> Note: `azure-sql-edge` used to be the only option to run `mssql` on `arm` macs. Now, thanks to _x86_64/amd64 Rosetta emulation on Apple Silicon_, we can use the full version. [Read about it here](https://devblogs.microsoft.com/azure-sql/development-with-sql-in-containers-on-macos/). If you want though, you can still use `azure-sql-edge` using the details below.

The `azure-sql-edge` image which contains a special version of SQL Server designed for IoT/Edge computing which can run on `arm`. There are some [unsupported features](https://learn.microsoft.com/en-us/azure/azure-sql-edge/features#unsupported-features) to be aware of, but is good enough for dev.

```bash
docker-compose -f docker-compose-mssql.yml -f docker-compose-mssql-arm.yml up -d
```

## Further Configuration

After creating an mssql container for the first time, you might want to set the following settings. [Reference 🔗](https://learn.microsoft.com/en-us/sql/linux/sql-server-linux-configure-mssql-conf)

**Disable Telemetry**

```bash
/opt/mssql/bin/mssql-conf set telemetry.customerfeedback false
```

**View Current Config**

```bash
cat /var/opt/mssql/mssql.conf
```
