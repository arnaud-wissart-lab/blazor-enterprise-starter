const reconnectModal = document.getElementById("bes-reconnect-modal");

if (reconnectModal) {
    reconnectModal.addEventListener("components-reconnect-state-changed", handleReconnectStateChanged);
}

function setHidden(element, isHidden) {
    element.hidden = isHidden;
}

function createReconnectContent() {
    reconnectModal.replaceChildren();

    const container = document.createElement("div");
    container.className = "components-reconnect-container";

    const animationElement = document.createElement("div");
    animationElement.id = "components-reconnect-animation";
    animationElement.className = "components-rejoining-animation";
    animationElement.setAttribute("aria-hidden", "true");
    animationElement.hidden = true;
    animationElement.append(document.createElement("div"), document.createElement("div"));

    const eyebrowElement = document.createElement("p");
    eyebrowElement.id = "components-reconnect-eyebrow";
    eyebrowElement.className = "components-reconnect-eyebrow";
    eyebrowElement.hidden = true;

    const titleElement = document.createElement("h2");
    titleElement.id = "components-reconnect-title";
    titleElement.className = "components-reconnect-title";

    const messageElement = document.createElement("p");
    messageElement.id = "components-reconnect-message";
    messageElement.className = "components-reconnect-message";

    const countdownElement = document.createElement("p");
    countdownElement.id = "components-reconnect-countdown";
    countdownElement.className = "components-reconnect-countdown";
    countdownElement.hidden = true;
    countdownElement.append("Nouvelle tentative dans ");

    const secondsElement = document.createElement("span");
    secondsElement.id = "components-seconds-to-next-attempt";
    countdownElement.append(secondsElement, " secondes.");

    const actionsElement = document.createElement("div");
    actionsElement.className = "components-reconnect-actions";

    const retryButton = document.createElement("button");
    retryButton.id = "components-reconnect-button";
    retryButton.type = "button";
    retryButton.hidden = true;
    retryButton.addEventListener("click", retry);

    const resumeButton = document.createElement("button");
    resumeButton.id = "components-resume-button";
    resumeButton.type = "button";
    resumeButton.hidden = true;
    resumeButton.addEventListener("click", resume);

    actionsElement.append(retryButton, resumeButton);
    container.append(animationElement, eyebrowElement, titleElement, messageElement, countdownElement, actionsElement);
    reconnectModal.append(container);

    reconnectModal.setAttribute("aria-labelledby", "components-reconnect-title");
    reconnectModal.setAttribute("aria-describedby", "components-reconnect-message");

    return {
        animationElement,
        eyebrowElement,
        titleElement,
        messageElement,
        countdownElement,
        retryButton,
        resumeButton
    };
}

function clearReconnectContent() {
    reconnectModal.replaceChildren();
    reconnectModal.removeAttribute("aria-labelledby");
    reconnectModal.removeAttribute("aria-describedby");
}

function applyStateContent(state) {
    const elements = createReconnectContent();

    switch (state) {
        case "show":
            elements.eyebrowElement.textContent = "Connexion";
            elements.titleElement.textContent = "Connexion interrompue";
            elements.messageElement.textContent = "Tentative de reconnexion au serveur en cours.";
            setHidden(elements.eyebrowElement, false);
            setHidden(elements.animationElement, false);
            break;
        case "retrying":
            elements.eyebrowElement.textContent = "Connexion";
            elements.titleElement.textContent = "Nouvelle tentative en cours";
            elements.messageElement.textContent = "La connexion n’est pas encore rétablie.";
            setHidden(elements.eyebrowElement, false);
            setHidden(elements.animationElement, false);
            setHidden(elements.countdownElement, false);
            break;
        case "failed":
            elements.eyebrowElement.textContent = "Connexion";
            elements.titleElement.textContent = "Connexion indisponible";
            elements.messageElement.textContent = "La session n’a pas pu être rétablie pour le moment.";
            elements.retryButton.textContent = "Réessayer";
            setHidden(elements.eyebrowElement, false);
            setHidden(elements.retryButton, false);
            break;
        case "paused":
            elements.eyebrowElement.textContent = "Session";
            elements.titleElement.textContent = "Session en pause";
            elements.messageElement.textContent = "Vous pouvez tenter de reprendre la session en cours.";
            elements.resumeButton.textContent = "Reprendre";
            setHidden(elements.eyebrowElement, false);
            setHidden(elements.resumeButton, false);
            break;
        case "resume-failed":
            elements.eyebrowElement.textContent = "Session";
            elements.titleElement.textContent = "Reprise impossible";
            elements.messageElement.textContent = "La session n’a pas pu être reprise. Vous pouvez réessayer.";
            elements.resumeButton.textContent = "Réessayer";
            setHidden(elements.eyebrowElement, false);
            setHidden(elements.resumeButton, false);
            break;
        default:
            clearReconnectContent();
            break;
    }
}

function handleReconnectStateChanged(event) {
    const state = event.detail.state;

    if (state === "hide") {
        document.removeEventListener("visibilitychange", retryWhenDocumentBecomesVisible);

        if (reconnectModal.open) {
            reconnectModal.close();
        }

        clearReconnectContent();
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
                clearReconnectContent();
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
            clearReconnectContent();
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
