function slist(target) {
    // Gets the list of players and adds the css class to it
    target.classList.add("slist");

    let items = target.getElementsByTagName("li"), current = null;

    for (let i of items) {
        //Make all items draggable
        i.draggable = true;

        // On drag start change color of all items except the one being dragged
        i.ondragstart = e => {
            current = i;
            for (let it of items) {
                if (it != current) { it.classList.add("hint"); }
            }
        };

        // Adds red highlight when drag enters the dropzone
        i.ondragenter = e => {
            if (i != current) { i.classList.add("active"); }
        };

        // Removes red highlight when drag leaves the dropzone
        i.ondragleave = () => i.classList.remove("active");

        // Removes all highlight when drag ends
        i.ondragend = () => {
            for (let it of items) {
                it.classList.remove("hint");
                it.classList.remove("active");
            }
        };

        // Prevents default action of the browser so i can implement mine
        i.ondragover = e => e.preventDefault();

        // On drop, if the item being dropped is not the same as the item being dragged, then swap them
        i.ondrop = e => {
            e.preventDefault();
            if (i != current) {
                // Gets the position of the current item and the item being dropped
                let currentpos = 0, droppedpos = 0;
                for (let it = 0; it < items.length; it++) {
                    if (current == items[it]){
                        currentpos = it; // current = item that is being dropped on
                    }
                    if (i == items[it]){
                        droppedpos = it; // i = dragged item
                    }
                }

                // Get the IDs of the contracts to be updated
                const currentContractId = current.getAttribute('data-ContractId');
                const droppedContractId = i.getAttribute('data-ContractId');

                $.ajax({ 
                    type: 'POST',
                    url: '/Contract/UpdatePositions',
                    data: {
                        currentContractId: currentContractId,
                        droppedContractId: droppedContractId
                    },
                    success: function (data) {
                        // Update the player positions dynamically

                        // Swap the html attributes of the two items
                        const tempHTML = current.innerHTML; // stores the current innerHTML of the current item (all html and csss attributes)
                        current.innerHTML = i.innerHTML;
                        i.innerHTML = tempHTML;

                        // Ensures that the players have the correct data-ContractId attribute
                        current.setAttribute("data-ContractId", droppedContractId);
                        i.setAttribute("data-ContractId", currentContractId);
                    },
                    error: function () {
                        alert("An error occurred while updating positions.");
                    }
                });


            }
        };
    }
}

 