const URL = 'ws://localhost:9000/Game'
var ws

// class Field {
//     segno = ''
//     field = []
    
//     constructor(campo, segno) {
//         var row1 = []
//         var row2 = []
//         var row3 = []
//         this.segno = segno;
//         for (let i = 0; i < campo.length; i++) {
//             var nome = ''
//             const element = campo[i];
//             if (element.hasChildNodes()) {
//                 var fchild = campo[i].firstChild
//                 var nomeCompleto = fchild.getAttribute('src')
//                 nome = nomeCompleto.split('/')[2].split('.')[0]        
//             }

//             if (i < 3) {
//                 row1.push(nome != '' ? nome : 'vuoto')
//             } else if (i >= 3 && i <= 5) {
//                 row2.push(nome != '' ? nome : 'vuoto')
//             } else {
//                 row3.push(nome != '' ? nome : 'vuoto')
//             }
//         }

//         this.field.push(row1, row2, row3)
//     }
// }

function connect() {
    ws = new WebSocket(URL)

    ws.onopen = handleOpen

    ws.onmessage = handleMsg
}

function handleOpen() {
    sendMsg(`|conn|new-${segnoUtente}`)
}

function handleMsg(msg) {
    var data = msg.data
    console.log('data:', data)

    if (data.startsWith('|field|')) {
        var segno = data.split('|')[2].split('-')[0]
        var pos = parseInt(data.split('|')[2].split('-')[1])
        printInField(pos, segno)
        setEventListenersFreeCell()
    }

    if (data.startsWith('|error|changedSymbol')) {
        segnoUtente = data.split('-')[1]
    }

    if (data.startsWith('|conn|accepted')) {
        userID = parseInt(data.split('-')[1])
        document.getElementById('userID').innerHTML = userID
    }

    if (data.includes('start')) {
        document.getElementById('divWait').setAttribute('hidden', true)
    }

    if (data.includes('wait')) {
        document.getElementById('divWait').removeAttribute('hidden')
    }

    if (data.includes('refused')) {
        document.getElementById('divRef').removeAttribute('hidden')
    }
}

function sendMsg(msg) {
    ws.send(msg)
}