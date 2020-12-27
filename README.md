# Cooking Projet
<br><br>
Il s'agit d'un projet académique dans lequel l'on devait par groupe de deux créer une application sur laquelle les clients peuvent commander à manger. En plus de construire une interface cliente, il nous fallait gérer la base de données s'actualisant après chaque commande et gérant les réapprovisionnement.<br>
Voici plus bas le sujet avec un peu plus de détail.<br><br> 

## La startup Cooking
Cette startup souhaite valider son nouveau concept de partage de cuisine, basé sur son site internet "notre petite
cuisine".<br>
**La partie "classique" du concept :**<br>
Le site propose des plats cuisinés. Les clients commandent ces plats par une interface (IHM) sur mobile (ios ou
android), Web ou autre… Puis les plats sont livrés aux clients par un service de livraison à vélo.
<br><br>**La partie "novatrice" : l'échange entre utilisateurs**<br>
Les clients qui commandent les plats sont aussi les cuisiniers qui peuvent proposer les recettes et gagner des points
(les cook) avec lesquels ils pourront payer leurs achats de repas sur notre site "notre petite cuisine".
Chaque client peut proposer ses recettes qui pourront être intégrées à la liste des recettes proposées par Cooking.
<br><br>
**Fonctionnement du concept :**
3 acteurs interagissent dans ce concept : les clients, les créateurs de recette, la société Cooking.<br>
Les clients:
 - Ils commandent les plats
 - Ils payent les plats commandés
Les créateurs de recette (CdR):
 - Ils fournissent les recettes
 - Ils sont rémunérés à chaque fois qu'une de leur recette est commandée.
 - Une remarque importante : ce sont des clients du service Cooking
 - Les CdR (non salariés) sont rémunérés en points leur permettant de commander des plats cuisinés.
Cooking:
 - Ils réalisent les recettes commandées par les clients
 - Ils gèrent l'approvisionnement des produits nécessaires
 - Ils organisent les livraisons des plats cuisinés
 - Ils encaissent les règlements des clients et rémunèrent les cuisiniers.
<br><br>
## Les informations détaillées :
Il vous est fournit ci-dessous les informations décrites par le client (la société Cooking)
Vous rajouterez toute information supplémentaire nécessaire à la réalisation des fonctionnalités demandées dans le
cahier des charges.<br>
Vous utiliserez les identifiants qui vous sembleront pertinents.<br><br>
• Un client a :
 - un nom
 - un numéro de téléphone.<br>
 
• Un créateur de recettes (CdR)
 - c'est un client particulier (tout CdR est d'office un client, mais tous les clients ne sont pas des CdR)<br>
 
• Une recette est constituée
 - d'un nom,
 - d'un type (descriptif en un mot)
 - d'une liste d'ingrédients (les "produits") et des quantités nécessaires (exprimée dans l'unité du
produit),
 - d'un descriptif ("un texte" de 256 caractères),
 - d'un prix de vente client exprimé en cook (fixé arbitrairement par le CdR à la création de la recette
(entre 10 et 40 cook) et d'une rémunération pour le cuisinier (fixée arbitrairement à 2 cook à la
création d'une recette).<br>

• Les produits :
 - un nom,
 - une catégorie (viande, poisson, légume…),
 - une unité de quantité applicable à ce produit (cette unité servant à la fois aux recettes et aux
commandes de réapprovisionnement),
 - un stock actuel,
 - un stock minimal (fixé à la création d'une recette à stock mini précédent/2 + 3 fois la quantité pour
cette nouvelle recette),
 - un stock maximum (fixé à la création d'une la recette à stock maxi précédent + 2 fois la quantité
pour cette nouvelle recette),
 - un nom de fournisseur (pas de gestion des prix d'achat dans ce PoC),
 - une référence fournisseur<br>
 
• Les fournisseurs :
 - un nom,
 - un numéro de téléphone.
