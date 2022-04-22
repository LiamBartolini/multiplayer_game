var cells = document.getElementsByClassName('cell')
var segnoUtente = ''
var userID
var moveNo = 0

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

    return false
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
    if (moveNo >= 3) {
        sendField()
    }

    moveNo++
    var target = e.target
    var img = document.createElement('img')
    img.setAttribute('src', `../img/${segnoUtente}.png`)
    target.appendChild(img)
    target.removeEventListener('click', handleCellClick)

    sendMsg(`|field|${segnoUtente}-${target.getAttribute('id')}`)
    waintingOtherMove()
}

function printInField(pos, segno) {
    var img = document.createElement('img')
    img.setAttribute('src', `../img/${segno}.png`)
    cells[pos].appendChild(img)
}

function waintingOtherMove() {
    for (let i = 0; i < cells.length; i++) {
        const element = cells[i];
        if (!element.hasChildNodes()) {
            element.removeEventListener('click', handleCellClick)
        }
    }
}

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