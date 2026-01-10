# ServerISO – Serveur ISO8583 sécurisé (TLS)

## Présentation

ServerISO est un serveur de paiement simulé implémentant le standard ISO 8583, utilisé dans les systèmes de paiement électroniques (cartes bancaires, POS, ATM et switchs de paiement).  
Le projet a pour objectif de fournir une plateforme de test des flux de paiement, du format ISO8583 et des communications sécurisées.

Le serveur permet de recevoir des messages ISO8583 via une connexion TCP sécurisée (TLS), de les parser, de les traiter, puis de générer automatiquement une réponse ISO conforme.

---

## Architecture du projet

Le projet est structuré autour d’un serveur principal et d’un moteur ISO8583 développé sur mesure.

- ServerApp  
  - Iso8583  
    - FieldType.cs : définition des types de champs ISO (FIXED, LLVAR, LLLVAR)  
    - IsoBitmap.cs : gestion du bitmap primaire  
    - IsoFieldSpec.cs : spécification des champs ISO8583  
    - IsoMac.cs : génération et vérification du MAC  
    - IsoMessage.cs : représentation d’un message ISO8583  
    - IsoPacker.cs : construction des messages ISO8583  
    - IsoParser.cs : parsing des messages ISO8583 entrants  
    - IsoResponseGenerator.cs : génération des réponses ISO (ex : 0210)  
  - Server.cs : serveur TCP sécurisé et logique métier ISO  
  - Program.cs : point d’entrée de l’application  
  - Server.Designer.cs : interface WinForms  

---

## Sécurité

La communication entre le client et le serveur est sécurisée via SSL/TLS 1.2.  
Le serveur utilise un certificat X509 chargé depuis le magasin de certificats Windows et rejette toute connexion non sécurisée.

---

## Fonctionnement du projet

Au démarrage, le serveur écoute sur un port TCP (par défaut 5000) et attend des connexions clientes sécurisées.  
Lorsqu’un client se connecte, un handshake TLS est effectué. Une fois la connexion sécurisée établie, le serveur peut recevoir des messages ISO8583.

Le message ISO reçu est analysé par le parser ISO8583 qui :
- lit le MTI
- interprète le bitmap
- extrait les champs présents

Les données extraites sont ensuite traitées par la logique métier.  
En fonction du type de message (ex : MTI 0200), le serveur génère une réponse appropriée (ex : MTI 0210) en ajoutant les champs nécessaires comme le code réponse, le RRN, la date/heure et le MAC.

La réponse ISO8583 est ensuite construite par le packer et envoyée au client via la connexion TLS.

---

## Flux ISO8583

Exemple de flux :
- Message entrant : 0200 (transaction financière)
- Message sortant : 0210 (réponse à la transaction)

---

## Champs ISO supportés

Le projet supporte notamment les champs suivants :
- Champ 2 : PAN  
- Champ 3 : Processing Code  
- Champ 4 : Amount  
- Champ 7 : Transmission Date & Time  
- Champ 11 : STAN  
- Champ 12 : Local Time  
- Champ 13 : Local Date  
- Champ 37 : RRN  
- Champ 39 : Response Code  
- Champ 41 : Terminal ID  
- Champ 49 : Currency Code  
- Champ 64 : MAC  

---

## Tests

Le serveur peut être testé à l’aide d’outils comme OpenSSL pour vérifier la connexion TLS.

Exemple :
openssl s_client -connect localhost:5000 -tls1_2

Les scénarios de test incluent :
- Transaction approuvée
- PAN invalide
- Montant invalide
- Message ISO mal formé
- MTI non supporté

---

## Technologies utilisées

- C# / .NET  
- TCP & SSLStream  
- ISO 8583  
- WinForms  
- OpenSSL pour les tests TLS  

---
