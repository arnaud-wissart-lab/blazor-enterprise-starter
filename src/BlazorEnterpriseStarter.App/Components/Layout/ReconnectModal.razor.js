const reconnectModal = document.getElementById("components-reconnect-modal");
const titleElement = document.getElementById("components-reconnect-title");
const messageElement = document.getElementById("components-reconnect-message");
const eyebrowElement = document.getElementById("components-reconnect-eyebrow");
const countdownElement = document.getElementById("components-reconnect-countdown");
const animationElement = document.getElementById("components-reconnect-animation");
const retryButton = document.getElementById("components-reconnect-button");
const resumeButton = document.getElementById("components-resume-button");

if (reconnectModal && titleElement && messageElement && eyebrowElement && countdownElement && animationElement && retryButton && resumeButton) {
    reconnectModal.addEventListener("components-reconnect-state-changed", handleReconnectStateChanged);
    retryButton.addEventListener("click", retry);
    resumeButton.addEventListener("click", resume);
}

function setHidden(element, isHidden) {
    element.hidden = isHidden;
}

function resetModalContent() {
    eyebrowElement.textContent = "";
    titleElement.textContent = "";
    messageElement.textContent = "";
    setHidden(eyebrowElement, true);
    setHidden(countdownElement, true);
    setHidden(animationElement, true);
    setHidden(retryButton, true);
    setHidden(resumeButton, true);
}

function applyStateContent(state) {
    resetModalContent();

    switch (state) {
        case "show":
            eyebrowElement.textContent = "Connexion";
            titleElement.textContent = "Connexion interrompue";
            messageElement.textContent = "Tentative de reconnexion au serveur en cours.";
            setHidden(eyebrowElement, false);
            setHidden(animationElement, false);
            break;
        case "retrying":
            eyebrowElement.textContent = "Connexion";
            titleElement.textContent = "Nouvelle tentative en cours";
            messageElement.textContent = "La connexion n’a pas encore été rétablie.";
            setHidden(eyebrowElement, false);
            setHidden(animationElement, false);
            setHidden(countdownElement, false);
            break;
        case "failed":
            eyebrowElement.textContent = "Connexion";
            titleElement.textContent = "Connexion indisponible";
            messageElement.textContent = "La session n’a pas pu être rétablie pour le moment.";
            retryButton.textContent = "Réessayer";
            setHidden(eyebrowElement, false);
            setHidden(retryButton, false);
            break;
        case "paused":
            eyebrowElement.textContent = "Session";
            titleElement.textContent = "Session en pause";
            messageElement.textContent = "Vous pouvez tenter de reprendre la session en cours.";
            resumeButton.textContent = "Reprendre";
            setHidden(eyebrowElement, false);
            setHidden(resumeButton, false);
            break;
        case "resume-failed":
            eyebrowElement.textContent = "Session";
            titleElement.textContent = "Reprise impossible";
            messageElement.textContent = "La session n’a pas pu être reprise. Vous pouvez réessayer.";
            resumeButton.textContent = "Réessayer";
            setHidden(eyebrowElement, false);
            setHidden(resumeButton, false);
            break;
        default:
            break;
    }
}

function handleReconnectStateChanged(event) {
    const state = event.detail.state;

    if (state === "hide") {
        document.removeEventListener("visibilitychange", retryWhenDocumentBecomesVisible);
        resetModalContent();

        if (reconnectModal.open) {
            reconnectModal.close();
        }

        return;
    }

    if (state === "rejected") {
        location.reload();
        return;
    }

    if (state === "failed") {
        document.addEventListener("visibilitychange", retryWhenDocumentBecomesVisible);
    }

    applyStateContent(state);

    if (!reconnectModal.open) {
        reconnectModal.showModal();
    }
}

async function retry() {
    document.removeEventListener("visibilitychange", retryWhenDocumentBecomesVisible);

    try {
        const successful = await Blazor.reconnect();

        if (!successful) {
            const resumeSuccessful = await Blazor.resumeCircuit();

            if (!resumeSuccessful) {
                location.reload();
            } else if (reconnectModal.open) {
                reconnectModal.close();
            }
        }
    } catch {
        document.addEventListener("visibilitychange", retryWhenDocumentBecomesVisible);
        applyStateContent("failed");
    }
}

async function resume() {
    try {
        const successful = await Blazor.resumeCircuit();

        if (!successful) {
            location.reload();
        } else if (reconnectModal.open) {
            reconnectModal.close();
        }
    } catch {
        applyStateContent("resume-failed");
    }
}

async function retryWhenDocumentBecomesVisible() {
    if (document.visibilityState === "visible") {
        await retry();
    }
}
