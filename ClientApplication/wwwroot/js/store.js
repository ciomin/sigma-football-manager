const cheapButton = document.getElementById("cheap-pack")
const regularButton = document.getElementById("regular-pack")
const expensiveButton = document.getElementById("expensive-pack")

const currency = document.getElementById("wallet")

const cardView = document.getElementById("card-view")

const cheapPack = {
    price: 500,
    rarity: 0,
    commonChance: 0.85,
    regularChance: 0.10,
    rareChance: 0.05
}

const regularPack = {
    price: 1000,
    rarity: 1,
    commonChance: 0.05,
    regularChance: 0.85,
    rareChance: 0.10
}

const expensivePack = {
    price: 2500,
    rarity: 2,
    commonChance: 0.05,
    regularChance: 0.15,
    rareChance: 0.80
}

cheapButton.addEventListener("click", function () { buyDeck(cheapPack) })
regularButton.addEventListener("click", function () { buyDeck(regularPack) })
expensiveButton.addEventListener("click", function () { buyDeck(expensivePack) })


function buyDeck(pack) {
    if (wallet < pack.price) {
        alert("You do not have enough money to buy this pack!");
    }
    else {
        wallet = wallet - pack.price;

        $.ajax({
            url: "/Home/updateMoney",
            type: "POST",
            data: { value: wallet },
            success: function (response) {
                console.log("Success:", response);
            },
            error: function (xhr) {
                console.log("Error:", xhr.responseText);
            }
        });

        currency.innerHTML = wallet;


        var cardsGot = selCards(pack.commonChance, pack.regularChance, pack.rareChance)
        console.log(cardsGot)
        playerInDb(cardsGot)
        displayCards(cardsGot)
    }
    
}

function selCards(cchance, rchance, echance) {
    var opened = [];
    var found = [];
    let tried;
    for (var i = 0; i < 5; i++) {
        var player;
        tried = closest(Math.random(), cchance, rchance, echance);
        if (tried == "commonPlayer") {
            player = cheapDeck[Math.floor(Math.random() * (cheapDeck.length))]
            if (found.includes(player)) {
                while (found.includes(player)) {
                    player = cheapDeck[Math.floor(Math.random() * (cheapDeck.length))]
                }
            }
            found.push(player)
            
            opened.push({
                fName : player.firstName,
                lName : player.lastName,
                atk : player.attacking,
                mct: player.midfieldControl,
                def : player.defending,
                pid : player.playerId,
                col: '#CD7F32'
            })
        }
        else if (tried == "regularPlayer") {
            player = regularDeck[Math.floor(Math.random() * (regularDeck.length))]
            if (found.includes(player)) {
                while (found.includes(player)) {
                    player = regularDeck[Math.floor(Math.random() * (regularDeck.length))]
                }
            }
            found.push(player)
            opened.push({
                fName: player.firstName,
                lName: player.lastName,
                atk: player.attacking,
                mct: player.midfieldControl,
                def: player.defending,
                pid : player.playerId,
                col: '#979A9A'
            })

        }
        else if (tried == "rarePlayer") {
            player = expensiveDeck[Math.floor(Math.random() * (expensiveDeck.length))]
            if (found.includes(player)) {
                while (found.includes(player)) {
                    player = expensiveDeck[Math.floor(Math.random() * (expensiveDeck.length))]
                }
            }
            found.push(player)
            opened.push({
                fName : player.firstName,
                lName : player.lastName,
                atk : player.attacking,
                mct : player.midfieldControl,
                def : player.defending,
                pid : player.playerId,
                col : '#FFD700'
            })
        }
        
    }
    return opened
    found = [];
    opened = [];
}

function clearCardView() {
    cardView.innerHTML = "";
}

function displayCards(plyrs) {
    var tmp = ""
    for (var i = 0; i < 5; i++) {
        tmp = tmp + "<div class=\"rectangle\" style=\"background-color:" + plyrs[i].col + "\" >" + plyrs[i].fName + "</br>" + plyrs[i].lName + "</br></br>Attacking:" + plyrs[i].atk + "</br>MidControl:" + plyrs[i].mct + "</br>Deffending:" + plyrs[i].def +" </div>"
    }
    cardView.innerHTML = "<div class=\"overlay\"><div style=\"position: fixed;left: 15%;top: 15%;width: 100 %;height: 100 %; \"> " + tmp + "</br><button id=\"close\" style=\"z-index:30;\" onclick=\"clearCardView()\">Back to shop</button></div></div>"
}

function playerInDb(plyrs) {
    var pidList = []
    for (var i = 0; i < 5; i++) {
        pidList.push(plyrs[i].pid)
    }
    $.ajax({
        url: "/Home/addPlayerTeam",
        type: "POST",
        data: { playerIds: pidList },
        success: function (response) {
            console.log("Success:", response);
        },
        error: function (xhr) {
            console.log("Error:", xhr.responseText);
        }
    });
}

function closest(check, n1, n2, n3) {
    if (Math.abs(check - n1) <= Math.abs(check - n2) && Math.abs(check - n1) <= Math.abs(check - n3)) return "commonPlayer";
    if (Math.abs(check - n2) < Math.abs(check - n1) && Math.abs(check - n2) < Math.abs(check - n3)) return "regularPlayer";
    if (Math.abs(check - n3) < Math.abs(check - n2) && Math.abs(check - n3) < Math.abs(check - n1)) return "rarePlayer";
}