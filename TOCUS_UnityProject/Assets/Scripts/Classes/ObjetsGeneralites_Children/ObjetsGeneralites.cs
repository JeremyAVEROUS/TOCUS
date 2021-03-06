﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Classe Parent des objets de la ville
public class ObjetsGeneralites : MonoBehaviour
{
    [HideInInspector] public RepertoireDesSprites repertoireSprite;     //Repertoire des sprites générales
    [HideInInspector] public LevelManager levelManager;                 //Script gérant le déroulé de la partie

    //Composants
    [HideInInspector] public Transform transformObjet;              //Transform de l'objet
    [HideInInspector] public Image imageObjet;                      //Composant image du bouton de l'objet
    [HideInInspector] public GameObject boutonGOObjet;              //GameObject du bouton de l'objet
    [HideInInspector] public Button buttonObjet;                    //Composant bouton de l'objet
    [HideInInspector] public GameObject healthBarMax_GO;            //GameObject de la barre de vie Max de l'objet
    [HideInInspector] public GameObject nbTextRessources_GO;        //GameObject du texte affichant le nombre de ressources de l'objet

    [HideInInspector] public Sprite spriteBaseObjet;        //Déclaré ici mais utilisé dans la classe enfant (exemple champBle)
    [HideInInspector] public int nbSpritesObjet;        //Déclaré ici mais utilisé dans la classe enfant (exemple champBle)

    public string nomObjet;
    public bool isInteractable;         //Peut-on intergir avec l'objet
    public Vector3 scaleObjet;

    public GameObject monGO;            //GameObject de l'objet


    //Panel Interaction avec les batiments
    [HideInInspector] public GameObject imageFlagJoueur;       //Image du drapeau du joueur si le batiment est réservé 
    public int indicePanelInterractBatiment;                    //Indice pour savoir quel panel afficher (construction tours, batiment, unité, Chateau)  
    public GameObject batimentConstruit;                        //GO du batiment construit à cet emplacement
    [HideInInspector] public GameObject[] _InterractionsDisponibles;     //Boutons dans le panel d'interactions des batiments possibles à la construction
    [HideInInspector] public Text panelInteractText;            //Texte du titre du Panel



    //Sprites des Unités en attente (utile pour Caserne, détruite aux autres)
    [HideInInspector] public GameObject spritesUnitesFiles;

    //Nombre d'unité pour amélioration
    [HideInInspector] public int uniteMinToUpgrade;


    //Fonction Constructeur de la classe. Attribue les valeurs aux variables communes à tous les types d'objets définies ci-dessus.
    public ObjetsGeneralites()
    {
        monGO = (GameObject)Instantiate(Resources.Load("Prefab/BatimentPrefab"));
        repertoireSprite = GameObject.Find("Main Camera").GetComponent<RepertoireDesSprites>();
        levelManager = GameObject.Find("Main Camera").GetComponent<LevelManager>();
        transformObjet = monGO.transform;
        transformObjet.GetChild(0).GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        boutonGOObjet = transformObjet.GetChild(0).GetChild(0).gameObject;
        nbTextRessources_GO = transformObjet.GetChild(0).GetChild(1).gameObject;
        healthBarMax_GO = transformObjet.GetChild(0).GetChild(2).gameObject;
        imageFlagJoueur = transformObjet.GetChild(0).GetChild(3).gameObject;
        imageObjet = boutonGOObjet.GetComponent<Image>();
        buttonObjet = boutonGOObjet.GetComponent<Button>();
        batimentConstruit = monGO.transform.GetChild(1).gameObject;
        spritesUnitesFiles = transformObjet.GetChild(2).gameObject;


        if (!levelManager.isModeDebug)
        {
            uniteMinToUpgrade = 4;
        } else
        {
            uniteMinToUpgrade = 2;
        }
    }

    //Constructeur des Batiments. Attribue les variables spécifiques à chaque batiment une fois les valeurs récupérées depuis le répertoire des sprites
    public void ConstructeursBatiment(Sprite _spriteBaseObjet, string _NomObjet, bool _IsInteractable)
    {
        spriteBaseObjet = _spriteBaseObjet;
        nomObjet = _NomObjet;
        imageObjet.sprite = spriteBaseObjet;
        CheckBoutonInteract(_IsInteractable);
    }


    //Permet de rendre actif ou non le bouton de l'objet
    public void CheckBoutonInteract(bool _IsInteractable)
    {
        isInteractable = _IsInteractable;
        buttonObjet.interactable = isInteractable;
    }

    //Adapte l'échelle des éléments enfants de l'objet
    public void ScalingChildElement(GameObject _ObjectToScale)
    {
        _ObjectToScale.GetComponent<RectTransform>().localScale = new Vector3(_ObjectToScale.GetComponent<RectTransform>().localScale.x / _ObjectToScale.transform.parent.parent.localScale.x, _ObjectToScale.GetComponent<RectTransform>().localScale.y / _ObjectToScale.transform.parent.parent.localScale.y, 0) / 2;
    }


    //Permet de changer le joueur actif.
    public void ChangeJoueurActif()
    {
        levelManager.tableauJoueurs[levelManager._JoueurActif - 1].DesactiveImageActionRound(levelManager.nbActionRealised);    //Retire une action au joueur actif

        //Vérifie le nouveau joueur actif
        if (levelManager._JoueurActif == 1)
        {
            levelManager._JoueurActif = 2;
        }
        else if (levelManager._JoueurActif == 2)
        {
            levelManager._JoueurActif = 1;
        }

        //Change le joueur actif
        levelManager.SetJoueurActif();


        //Lève l'interaction possible avec un batiment
        if (!levelManager.isModeDebug)
        {
            InterractBatiment(false);
        }

        //Gère le round
        levelManager.GestionActionRound();



        //Réactive les objets si on reprend un nouveau round
        if(levelManager.nbActionRealised == 0)
        {
            levelManager._ChampBle.CheckBoutonInteract(true);
            levelManager._MineFer.CheckBoutonInteract(true);
            levelManager._CarrierePierre.CheckBoutonInteract(true);
            levelManager._RecolteBois.CheckBoutonInteract(true); 
            levelManager._CampMilitaire.CheckBoutonInteract(true);
            levelManager._ChantierTours.CheckBoutonInteract(true);
            levelManager._ChantierBTP.CheckBoutonInteract(true);
            levelManager._Chateau.CheckBoutonInteract(true);
            if (GameObject.FindGameObjectsWithTag("EmplacementBTP").Length > 0)
            {
                GameObject[] emplacementBatiment = GameObject.FindGameObjectsWithTag("EmplacementBTP");
                for(int ii=0;ii< GameObject.FindGameObjectsWithTag("EmplacementBTP").Length; ii++)
                {
                    emplacementBatiment[ii].transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Button>().interactable = true;
                }
            }
            levelManager.joueur1.ActiveImagesActionRound();
            levelManager.joueur2.ActiveImagesActionRound();
        }
    }


    //Gère l'interactabilité avec le bouton du batiment en fonction du batiment !
    public void InterractBatiment(bool _IsInteractable)
    {
        if (levelManager.myEventSystem.currentSelectedGameObject != null)
        {
            if (levelManager.myEventSystem.currentSelectedGameObject.transform.parent.parent.tag == "Batiment")
            {
                CheckBoutonInteract(_IsInteractable);
            }
            else if (levelManager.myEventSystem.currentSelectedGameObject.transform.parent.parent.tag == "PanelConstruct")
            {
                if (levelManager.myEventSystem.currentSelectedGameObject.transform.parent.parent.name == "PanelConstructUnites")
                {
                    levelManager._CampMilitaire.CheckBoutonInteract(_IsInteractable);
                }
                else if (levelManager.myEventSystem.currentSelectedGameObject.transform.parent.parent.name == "PanelChateau")
                {
                    levelManager._Chateau.CheckBoutonInteract(_IsInteractable);
                }

            }
            else if (levelManager.myEventSystem.currentSelectedGameObject.transform.parent.parent.parent != null && levelManager.myEventSystem.currentSelectedGameObject.transform.parent.parent.parent.tag == "EmplacementTour")
            {
                levelManager._ChantierTours.CheckBoutonInteract(_IsInteractable);
            }
            else if (levelManager.myEventSystem.currentSelectedGameObject.transform.parent.parent.tag == "EmplacementBTP")
            {
                levelManager._ChantierBTP.CheckBoutonInteract(_IsInteractable);
            }
            else if (levelManager.myEventSystem.currentSelectedGameObject.transform.parent.parent.parent != null && levelManager.myEventSystem.currentSelectedGameObject.transform.parent.parent.parent.tag == "EmplacementBTP")
            {
                levelManager.myEventSystem.currentSelectedGameObject.GetComponent<Button>().interactable = _IsInteractable;
            } else if(levelManager.myEventSystem.currentSelectedGameObject.name == "ButtonEndChoice")
            {

                levelManager._ChantierTours.CheckBoutonInteract(_IsInteractable);
            }
        } else if(levelManager.buttonSelectedTemp.name == "Mortier")
        {
            levelManager.buttonSelectedTemp.transform.GetChild(0).GetChild(0).GetComponent<Button>().interactable = _IsInteractable;
        }
    }


    //Ouvre le panel de gameplay du batiment
    public void OuverturePanelInterractBatiment(int indicePanel)
    {
        levelManager.panelParentBatimentInterract.gameObject.SetActive(true);
        for (int ii = 0; ii < levelManager.panelParentBatimentInterract.childCount - 1; ii++)
        {
            if (ii == indicePanel)
            {
                levelManager.panelParentBatimentInterract.GetChild(ii).gameObject.SetActive(true);
            }
            else
            {
                levelManager.panelParentBatimentInterract.GetChild(ii).gameObject.SetActive(false);
            }
        }

        if (transformObjet.name == "CampMilitaire")
        {
            //Bouton amélioration se désactive de base
            levelManager._CampMilitaire._InterractionsDisponibles[1].GetComponent<Button>().interactable = false;

            //Pour le joueur actif, on regarde le nombre d'unités recrutées pour chaque type. Si l'une des valeurs est supérieure au nombre d'unités permettant l'upgrade, on active le bouton
            for (int ii = 0;ii< levelManager.tableauJoueurs[levelManager._JoueurActif - 1]._NbUnites.Length; ii++)
            {
                if(levelManager.tableauJoueurs[levelManager._JoueurActif - 1]._NbUnites[ii] >= uniteMinToUpgrade)
                {
                    //levelManager._CampMilitaire.indUniteCanUpgrade = ii;
                   levelManager._CampMilitaire._InterractionsDisponibles[1].GetComponent<Button>().interactable = true;
                    levelManager._CampMilitaire.boutonsUnitesToUpgrade[ii].GetComponent<Button>().interactable = true;
                    //break;
                } else
                {
                    levelManager._CampMilitaire.boutonsUnitesToUpgrade[ii].GetComponent<Button>().interactable = false;
                }
            }

            


            //Bouton 1 : Recruter unité
            /*for (int ii = 0; ii < _InterractionsDisponibles.Length; ii++)
            {
                //En fonction du joueur actif, on gère la sprite de l'unité affichée ainsi que son orientation pour le Camp Militaire
                _InterractionsDisponibles[ii].GetComponent<Image>().sprite = repertoireSprite.unitesJoueurData[ii].uniteSpriteBase[levelManager._JoueurActif - 1];
                if(levelManager._JoueurActif == 1)
                {
                    _InterractionsDisponibles[ii].GetComponent<RectTransform>().localScale = new Vector2(_InterractionsDisponibles[ii].GetComponent<RectTransform>().localScale.x, _InterractionsDisponibles[ii].GetComponent<RectTransform>().localScale.y);
                    for (int jj = 0; jj < _InterractionsDisponibles[ii].transform.childCount; jj++)
                    {
                        _InterractionsDisponibles[ii].transform.GetChild(jj).GetComponent<RectTransform>().localScale = new Vector2(_InterractionsDisponibles[ii].transform.GetChild(jj).GetComponent<RectTransform>().localScale.x, _InterractionsDisponibles[ii].transform.GetChild(jj).GetComponent<RectTransform>().localScale.y);
                        _InterractionsDisponibles[ii].transform.GetChild(jj).GetComponent<RectTransform>().anchoredPosition = new Vector2(_InterractionsDisponibles[ii].transform.GetChild(jj).GetComponent<RectTransform>().anchoredPosition.x, _InterractionsDisponibles[ii].transform.GetChild(jj).GetComponent<RectTransform>().anchoredPosition.y);
                    }
                } else if(levelManager._JoueurActif == 2)
                {
                    _InterractionsDisponibles[ii].GetComponent<RectTransform>().localScale = new Vector2(-_InterractionsDisponibles[ii].GetComponent<RectTransform>().localScale.x, _InterractionsDisponibles[ii].GetComponent<RectTransform>().localScale.y);
                    for(int jj = 0; jj < _InterractionsDisponibles[ii].transform.childCount; jj++)
                    {
                        _InterractionsDisponibles[ii].transform.GetChild(jj).GetComponent<RectTransform>().localScale = new Vector2(-_InterractionsDisponibles[ii].transform.GetChild(jj).GetComponent<RectTransform>().localScale.x, _InterractionsDisponibles[ii].transform.GetChild(jj).GetComponent<RectTransform>().localScale.y);
                        _InterractionsDisponibles[ii].transform.GetChild(jj).GetComponent<RectTransform>().anchoredPosition = new Vector2(-_InterractionsDisponibles[ii].transform.GetChild(jj).GetComponent<RectTransform>().anchoredPosition.x, _InterractionsDisponibles[ii].transform.GetChild(jj).GetComponent<RectTransform>().anchoredPosition.y);
                    }
                }
            }*/
        }
    }









}
