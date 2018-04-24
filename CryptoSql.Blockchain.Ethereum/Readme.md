Safelow gas price is around 1 gwei right now - https://ethgasstation.info/

1 Gwei = 1e-9 ETH = 92.5e-9 CAD

Can store data in one of two ways:

* As contract storage
  * Cost is 20k gas for each 256 bit word
  * 256 bits can store 32 characters under ASCII
  * So cost per character is 625 gas = 625e-9 ETH
  * Still need to investigate contract overhead but it is at least 15%
  * Still need to send the data over transaction, so overhead from that

* As transaction storage (2 ways) 
  * One large transaction holds entire database
    * Requires sending entire database on every update
	* Maximum gas per transaction is ~6M
	* Cost per byte is 68 so per character under ASCII
	* Cost is 21000 to send transaction
	* Maximum size is ~100kB which would cost ~7M gas to update
  * Break the database down to transactions and store references
    * Each transaction contains just one row
	* Rows referenced by table transactions, which are only updated when a row is dropped or added
	* Tables are referenced by a master transaction which is only updated when tables are added/dropped

Benefits/drawbacks overview
* Contract
  * +Most native way of doing things
  * +Max database size is theoretically infinite
  * +Costs do not scale with database size
  * -Transaction overhead likely makes it less cost-effective than referenced transactions
  * -Values are only 32 bytes so always pay for that even if data is only 1 byte (e.g. bool)
  * -Since there's a strict 32-byte key to 32-byte value relation, structuring will be difficult
* Transaction
  * Monolithic
    * +Easiest by far
	* +All optimization for storage can be done with existing libraries
	* -Database cannot exceed ~100kB
	* -Cost to update any part of database directly proportional to database size
  * Referenced
    * +Likely cheapest
	* +Easier than contract
* All free to retrieve data
* All likely perform similar

Launch geth under lsxx with command `geth --rinkeby --rpc` for testnet