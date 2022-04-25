const URL = 'ws://localhost:9000/Game'
var ws

function connect() {
    ws = new WebSocket(URL)

    ws.onopen = handleOpen

    ws.onmessage = handleMsg

    ws.onclose = handleClose
}

function handleOpen() {
    sendMsg(`|conn|new-${segnoUtente}`)
}

function handleClose() {
    sendMsg(`|quit|${userID}`)
}

function handleMsg(msg) {
    var data = msg.data
    console.log('data:', data)

    if (data == "|check|") {
        sendField()
    }

    if (data.startsWith('|move|')) {
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

    if (data === '|quit|') {
        document.getElementById('divQuit').removeAttribute('hidden')
        otherClientQuit()
    }

    if (data === '|restart|') {
        document.getElementById('divRestart').removeAttribute('hidden')
        document.getElementById('divWon').setAttribute('hidden', true)
        document.getElementById('divLose').setAttribute('hidden', true)
        restarted = true
    }

    if (data === "|draw|") {
        stopGame()
        document.getElementById('divDraw').removeAttribute('hidden')
    }

    if (data == '|won|') {
        stopGame()
        document.getElementById('divWon').removeAttribute('hidden')
    }
    
    if (data == '|lost|') {
        stopGame()
        document.getElementById('divLose').removeAttribute('hidden')
    }
}

function sendMsg(msg) {
    ws.send(msg)
}