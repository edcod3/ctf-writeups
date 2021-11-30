// SPDX-License-Identifier: MIT
pragma solidity ^0.6.0;

contract UnknownOrigin {
    address public owner;

    constructor() public {
          owner = msg.sender;
    }
      
    modifier onlyOwned () {
          require(msg.sender != tx.origin);
          _;
    }

    function updateOwner (address _newOwner) public onlyOwned {
          owner = _newOwner;
    }
}