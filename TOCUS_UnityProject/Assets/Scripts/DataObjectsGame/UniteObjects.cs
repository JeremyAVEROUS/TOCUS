﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unite_Objects : ScriptableObject
{
    public string uniteNom;                 //Nom de l'unité

    //Variables des différents personnages (à renommer)
    public Vector2 uniteBoxCollSize;            //Taille de la boxCollider de l'unité
    public Vector2 uniteScale;                  //Scale de l'unité
    public Vector2 unitePositionCanvas;         //Position du Canvas de la barre de vie de l'unité
    public Vector2 uniteScaleCanvas;            //Scale du Canvas de l'unité


    //Caractéristiques
    public enum typeVitalite { Enorme, Importante, Normale, Faible, _1PV };       //Liste des types de vitalité
    public typeVitalite uniteTypeVitalite;      //Type de vitalité de l'unité

    public enum typeInitiative { First, Rapide, Normale, Lente, Last };       //Liste des types d'Initiative
    public typeInitiative uniteTypeInitiative;              //Type de l'initiative de l'unité

    public enum typeDegats { Enorme, OneShot, Important, Normal, Faible, _1PV };       //Liste des types de dégats
    public typeDegats uniteTypeDegats;          //Type de dégats de l'unité
}




