# 🔐 ServerISO – Serveur TLS 1.2 pour Trames de Paiement

## 📌 Présentation

**ServerISO** est une application serveur développée en **C# (.NET / WinForms)** implémentant un **serveur TCP sécurisé via TLS 1.2**.

Il permet de recevoir, valider et traiter des **trames de paiement inspirées du standard ISO 8583**, puis de retourner une réponse normalisée.

Le serveur est conçu pour :

* être accessible **en local ou via Internet**
* garantir la **sécurité des échanges (TLS 1.2)**
* simuler un **serveur d’autorisation de paiement**

---

## 🎯 Objectifs du projet

* Implémenter un **handshake TLS 1.2 sécurisé**
* Traiter des trames de paiement structurées
* Valider les champs (PAN, montant, format)
* Retourner des codes réponse ISO
* Être testable via **OpenSSL, curl, Hercules, mobile, etc.**

---

## 🧱 Architecture

```
Client (Mobile / OpenSSL / App)
        |
        | TLS 1.2
        v
┌─────────────────────────┐
│      ServerISO          │
│  TCPListener + SslStream│
└─────────────────────────┘
```

---

## 🛠️ Technologies utilisées

* C# .NET (WinForms)
* TcpListener / TcpClient
* SslStream
* TLS 1.2
* X509Certificate2
* OpenSSL (tests)
* Git / GitHub

---

## 🚀 Démarrage du serveur

### 🔹 Configuration réseau

* **IP locale** : `192.168.1.9`
* **Port** : `5000`
* **Protocole** : TCP
* **Sécurité** : TLS 1.2 uniquement

Le serveur démarre automatiquement à l’exécution de l’application.

---

## 🔐 Certificat TLS

* Certificat **auto-signé**
* Stocké dans :

```
Certificats → Ordinateur local → Personnel
```

* Subject attendu :

```
CN=localhost
```

⚠️ En production, utiliser un certificat signé par une autorité reconnue.

---

## 📡 Format des trames

### 📥 Requête (exemple)

```
0200|4000001234567890|000000|000000010000|123456
```

| Champ           | Description            |
| --------------- | ---------------------- |
| 0200            | Message Type Indicator |
| PAN             | Numéro de carte        |
| Processing Code | Code traitement        |
| Amount          | Montant                |
| STAN            | Numéro de transaction  |

---

### 📤 Réponse

```
0210|00|APPROVED
```

| Code | Signification  |
| ---- | -------------- |
| 00   | APPROVED       |
| 13   | INVALID AMOUNT |
| 14   | INVALID PAN    |
| 30   | FORMAT ERROR   |

---

## 🧠 Règles de validation

* MTI doit commencer par `0200`
* PAN numérique ≥ 13 chiffres
* Montant numérique
* Structure exacte en 5 champs

---

## 🔒 Sécurité réseau

### Autoriser le port (Windows)

```powershell
netsh advfirewall firewall add rule name="TLS Server 5000" dir=in action=allow protocol=TCP localport=5000
```

### Accès public (Internet)

1. Récupérer l’IP publique

```bash
curl https://api.ipify.org
```

2. Configurer le **Port Forwarding** sur le routeur

```
Port externe : 5000
IP interne   : 192.168.1.9
Port interne : 5000
Protocole    : TCP
```

---

## 🧪 Tests avec OpenSSL

```bash
openssl s_client -connect IP_PUBLIQUE:5000 -tls1_2
```

Handshake attendu :

* TLSv1.2
* Cipher sécurisé (AES-GCM)
* Certificat serveur affiché

---
