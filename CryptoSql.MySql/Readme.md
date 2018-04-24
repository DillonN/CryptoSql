## String of commands used to test MySQL

`-A` switch tells MySQL to not prefetch table values which CryptoSQL does not yet support

```
mysql -h <host> -P <port> -u dillon -A
```

Turning off autocommit allows us to measure speeds for offline and online operations independently

```
set autocommit = 0;
```

```
create database test;
```

Select the test database

```
use test;
```

Create a model table

```
create table test (
    id INT(11),
    firstname VARCHAR(16),
    lastname VARCHAR(16),
    address VARCHAR(128),
    products INT(11)
);
```

The next command inserts 200 rows and is located in [another file](InsertCommand.txt)
It is repeated 3 times with commit timings, and after each a data load is timed for the blockchain backing

See how fast all rows can be retrieved

```
select * from test;
```

See how fast the table can be searched for values and update

```
update test 
set firstname = "Meika"
where lastname = "Audie";
```

See how fast the table can be emptied

```
delete from test;
```

The session is then exited, with Ctrl-C. The last delete operation does is not committed so the table is still populated.