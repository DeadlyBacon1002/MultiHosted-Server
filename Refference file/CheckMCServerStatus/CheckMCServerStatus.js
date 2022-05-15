const util = require('minecraft-server-util');

util.status('play.hypixel.net')
    .then((result) => {
        // Server is online
        console.log("Penis but first");
    })
    .catch((error) => {
        // Server is offline
        console.log("Penis");
    });