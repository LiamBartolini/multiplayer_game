var cells = document.getElementsByClassName('cell')
var segnoUtente = ''
var userID
var restarted = false

document.getElementById('btnSegno')
    .addEventListener('click', function() {
        segnoUtente = document.getElementById('slcSegno').value        
        removeInput()
        connect()
        game()
    })

function removeInput() {
    var btn = document.getElementById('btnSegno')
    btn.parentNode.removeChild(btn)

    var lbl = document.getElementById('lblSegno') 
    lbl.parentNode.removeChild(lbl)

    var slc = document.getElementById('slcSegno')
    slc.parentNode.removeChild(slc)
}

function game() {
    setEventListeners()
}

function setEventListeners() {
    for (let i = 0; i < cells.length; i++) {
        const element = cells[i]
        if (segnoUtente != '') {
            element.addEventListener('click', handleCellClick)
        }
    }
}

function handleCellClick(e) {
    if (restarted) {
        document.getElementById('divRestart').setAttribute('hidden', true)
    }

    var target = e.target
    var img = document.createElement('img')
    img.setAttribute('src', `../img/${segnoUtente}.png`)
    target.appendChild(img)

    // togliere la possibilità di cliccare sulla stessa casella due volte
    target.removeEventListener('click', handleCellClick)

    sendMsg(`|move|${segnoUtente}-${target.getAttribute('id')}`)
    waintingOtherMove()
}

// serve per stampare la mossa dell'altro client sullo schermo
function printInField(pos, segno) {
    var img = document.createElement('img')
    img.setAttribute('src', `../img/${segno}.png`)
    cells[pos].appendChild(img)
}

// blocca il campo in attesa della mossa dell'altro client
function waintingOtherMove() {
    for (let i = 0; i < cells.length; i++) {
        const element = cells[i];
        if (!element.hasChildNodes()) {
            element.removeEventListener('click', handleCellClick)
        }
    }
}

// imposta gli event listener sulle caselle ancora non utilizzate
function setEventListenersFreeCell() {
    for (let i = 0; i < cells.length; i++) {
        const element = cells[i];
        if (!element.hasChildNodes()) {
            element.addEventListener('click', handleCellClick)
        }
    }
}

function sendField() {
    var arr = []
    for (let i = 0; i < cells.length; i++) {
        const element = cells[i];
        if (element.hasChildNodes()) {
            var child = element.firstChild
            var fqn = child.getAttribute('src')
            var name = fqn.split('/')[2].split('.')[0]
            arr.push(name)
        } else {
            arr.push('')
        }
    }

    sendMsg(`|check|${arr}`)
}

function stopGame() {
    for (let i = 0; i < cells.length; i++) {
        const element = cells[i];
        element.removeEventListener('click', handleCellClick)
    }

    var div = document.getElementById('restartGameDiv')
    var btn = document.createElement('button')
    var lbl = document.createElement('label')

    btn.addEventListener('click', handleRestartGame)
    btn.innerText = 'Restart game'
    btn.setAttribute('id', 'btnRestart')

    lbl.setAttribute('id', 'lblRestart')
    lbl.innerHTML = 'Restart game: '

    div.appendChild(lbl)
    div.appendChild(btn)

    var btnStop = document.createElement('button')
    btnStop.setAttribute('id', 'btnStop')
    btnStop.innerText = 'Stop playing'
    btnStop.addEventListener('click', handleStopGame)

    var lblStop = document.createElement('label')
    lblStop.setAttribute('id', 'lblStop')
    lblStop.innerText = 'Stop playing: '

    document.getElementById('stopPlayingDiv').append(lblStop, btnStop)
}

function handleRestartGame() {
    // pulire il campo
    emptyField()
    
    // dire al server che riparte una partita
    sendMsg('|restart|')

    // riattivare gli event listener
    game()

    document.getElementById('divRestart').setAttribute('hidden', true)
    document.getElementById('divDraw').setAttribute('hidden', true)
    document.getElementById('divWon').setAttribute('hidden', true)
    document.getElementById('divLose').setAttribute('hidden', true)

    document.getElementById('btnRestart').remove()
    document.getElementById('lblRestart').remove()
    document.getElementById('btnStop').remove()
    document.getElementById('lblStop').remove()
}

function emptyField() {
    for (let i = 0; i < cells.length; i++) {
        const element = cells[i];
        if (element.hasChildNodes()) {
            element.removeChild(element.lastChild)
        }
    }
}

function handleStopGame() {
    handleClose();
    var div = document.getElementById('ilDivPotente')
    div.remove()
}

function otherClientQuit() {
    emptyField()

    document.getElementById('divRestart').setAttribute('hidden', true)
    document.getElementById('divDraw').setAttribute('hidden', true)
    document.getElementById('divLose').setAttribute('hidden', true)
    document.getElementById('divWon').setAttribute('hidden', true)
    document.getElementById('divQuit').setAttribute('hidden', true)

    document.getElementById('btnRestart').remove()
    document.getElementById('lblRestart').remove()
    document.getElementById('btnStop').remove()
    document.getElementById('lblStop').remove()

    document.getElementById('divWait').removeAttribute('hidden')
}