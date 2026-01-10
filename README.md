💳 ServerISO – Serveur ISO8583 sécurisé (TLS)
📌 Présentation

ServerISO est un serveur de paiement simulé implémentant le standard ISO 8583, utilisé dans les systèmes de paiement électroniques (cartes bancaires, switchs, POS, ATM).

Le projet fournit :

🔐 Un serveur TCP sécurisé en TLS 1.2

🧠 Un moteur ISO8583 custom (parser, packer, bitmap, MAC)

🔁 Une génération automatique de réponses ISO

🧪 Un environnement idéal pour tests des flux de paiement

🏗️ Architecture du projet
PaiementApp/
│
├── ServerApp/
│   ├── Iso8583/
│   │   ├── FieldType.cs              # Types de champs ISO (FIXED, LLVAR, LLLVAR)
│   │   ├── IsoBitmap.cs              # Gestion du bitmap primaire
│   │   ├── IsoFieldSpec.cs           # Définition des champs ISO
│   │   ├── IsoMac.cs                 # Calcul du MAC (Message Authentication Code)
│   │   ├── IsoMessage.cs             # Modèle du message ISO8583
│   │   ├── IsoPacker.cs              # Construction du message ISO
│   │   ├── IsoParser.cs              # Parsing du message ISO entrant
│   │   └── IsoResponseGenerator.cs   # Génération des réponses ISO (0210, etc.)
│   │
│   ├── Server.cs                     # Serveur TCP + TLS + logique ISO
│   ├── Program.cs                    # Point d’entrée de l’application
│   └── Server.Designer.cs            # UI WinForms
│
├── .gitignore
└── README.md
🔐 Sécurité

Communication sécurisée via SSL/TLS 1.2

Certificat X509 chargé depuis le Windows Certificate Store

Rejet automatique des clients non sécurisés

🔄 Fonctionnement global
1️⃣ Démarrage du serveur

Écoute sur le port 5000

TLS 1.2 activé

Attente des connexions clientes

🔐 SERVEUR SSL TLS 1.2 DÉMARRÉ
📡 Écoute sur 0.0.0.0:5000

2️⃣ Réception du message ISO8583

Le message ISO est reçu via TCP sécurisé

Il est parsé par IsoParser

Les champs sont extraits grâce au bitmap

3️⃣ Traitement métier

Vérification des champs obligatoires

Génération du MTI de réponse

Génération automatique de :

Response Code (field 39)

RRN (field 37)

Date / Heure

MAC (field 64)

4️⃣ Envoi de la réponse ISO

Construction du message avec IsoPacker

Envoi au client via le canal TLS
Tests
Test TLS avec OpenSSL
openssl s_client -connect localhost:5000 -tls1_2

Scénarios couverts

✅ Transaction approuvée

❌ PAN invalide

❌ Montant invalide

❌ Format ISO incorrect

❌ MTI non supporté

🧩 Champs ISO supportés
Champ	Description
2	PAN
3	Processing Code
4	Amount
7	Transmission DateTime
11	STAN
12	Local Time
13	Local Date
37	RRN
39	Response Code
41	Terminal ID
49	Currency
64	MAC
🚀 Technologies utilisées

C# (.NET)

TCP / SSLStream

ISO 8583

WinForms

OpenSSL (tests)
