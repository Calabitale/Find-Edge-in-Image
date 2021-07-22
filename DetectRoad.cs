using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//I can use this to create multiple searches at once just have another object create this as many times as I want

[System.Serializable]
public class DetectRoad : MonoBehaviour
{

    public Vector3 down;
    //public Vector2 pixeluv;

    public RaycastHit hit;

    public GameObject textureobj;
    public Texture2D OriginalTexture;

    public Color roadcolor;
    public Color gotColor;
    public Color CheckColor;

    public float infinitetimedelay;

    public GameObject clone;
    public Texture2D cloony;

    [SerializeField] private Texture2D cloneCloony;

    //I need this to automatically find the texture on the target object.
    //public Texture2D callclone;
    //public Texture2D testclone;

    //doesnt fucking work for some reason, will never know why
    //public Material Doofus;
    //public Material Doofuscopy;

    //public int rootroadposx;
    //public int rootroadposy;

    public int RoadWidth;

    public int Squasize;

    public bool EdgePos;
    public bool OutsideEdgepos;
    public bool FoundEdge;
    public bool MoveAlongEdgeSuccess;

    public int OutsideEdgeXpoint;
    public int OutsideEdgeYpoint;

    public float checkthis;

    public float checkRGB;
    public float resultRGB;

    public float originalRGB;

    public float temptesterfuckingthing;


    public bool MoveAlongEdgeSlight = false;
    public bool NearestColorTrue = false;

    public bool MostDifferentTrue = false;

    public int CheckCounter;


    public Camera cam;

    //public Vector2 

    //Better to use this than all the booleans//
    //NearestSearch search's for all the colours and keeps searching till false, while nearestedgesearch return true when it finds the three colours that are nearest to the colour
    enum SearchState
    {
        RoadSearch,
        OutsideEdgeSearch,
        ActualEdgeSearch,
        OtherSideSearch,
        NearestSearch,        
        MoveAlongEdge,
        DoNothing
    }

    SearchState CurrentSearch;

    //NearestColorMultiples search's for all the colours and keeps searching till only one square is false, while NearestColor return true when it finds the three colours that are nearest to the colour
    //NearestColour is useful for just finding a colour that is near to a colour that i want and will return true instantly, whereas NearestColourMultiples is good for searching for an actual edge
    //It keeps going till thereas a negivative basically
    enum SearchType
    {
        NearestColour,
        NearestColourMultiples,
        ExactMatch,
        SameColourTillDifference,
        MostDifferent,
        MoveAlongEdge
    }

    //SearchType CurrentType;

    //This is the square creation thingy i dont know
    //public List<int> squarepoints = new List<int>();

    //TextureDraw drawling;
    //enum RoadFindState { FindRoad, FindWidth, FindLength, Nothing};

    //RoadFindState CurrRoadState;

    //Has to be public for some reason I dont know why
    //public struct Squarepoint
    //{
        //public int xpoint;
        //int ypoint;

    //}
    [System.Serializable]
    public class SquarepointsPosition//(int x, int y)
    {
        public int xpoint;// These were bullshit and not needed they made it so I couldn't even see the class in the inspector { get; set; }
        public int ypoint;// { get; set; }
    }

    //This is a list of squarepoints of the struct type squarepoint
    //[System.Serializable]
    public List<SquarepointsPosition> squarepoints = new List<SquarepointsPosition>();

    //RootPosition is from where I want to start from the root somewhere a new 
    
    public SquarepointsPosition RootXYposition;
    public SquarepointsPosition EdgeXYposition;
    public SquarepointsPosition CurrXYposition;

    public SquarepointsPosition StartEdgePosition;
    public SquarepointsPosition SecondEdgePosition;
    public SquarepointsPosition EndEdgePosition;
    public SquarepointsPosition MiddleEdgePosition;
    public SquarepointsPosition CurrentEdgePosition;

    // Use this for initialization
    void Start()
    {
        //I copied the original texture by accident woot!  Well not exactly I dont think this is a reference to the original texture that is all, cloony is an instantiated copy of this texture
        OriginalTexture = textureobj.GetComponent<Renderer>().material.mainTexture as Texture2D;

        CurrentSearch = SearchState.DoNothing;

        //I need to make sure this doesnt go to Zero or into minus numbers
        Squasize = 1;

        CheckColor = Color.magenta;

        OutsideEdgepos = false;
        EdgePos = false;
        FoundEdge = false;

        //For selecting the colour I could use a rattrace thingy from a mouse pointer, so that you can select the colour you want to designate as the road
        //directly on the map
        //Now it should be able to take any texture size ah ha!
        roadcolor = OriginalTexture.GetPixel(OriginalTexture.width / 2, OriginalTexture.width / 2);
        //roadcolor = OriginalTexture.GetPixel(512, 512);

        gotColor = Color.white;

        infinitetimedelay = 100.0f;

        //This is a copy of the original texture which I will use to get and set primarily, this is somewhere i dont know where, in memory but not set to the object
        cloony = Instantiate(textureobj.GetComponent<Renderer>().material.mainTexture as Texture2D);
        //This is the copy that I will use to see what is going on I will only set on this texture
        cloneCloony = Instantiate(textureobj.GetComponent<Renderer>().material.mainTexture as Texture2D);

        //textureobj.GetComponent<Renderer>().material.mainTexture = cloony;
        textureobj.GetComponent<Renderer>().material.mainTexture = cloneCloony; 
        

        //These dont need to be in updated I dont understand this I'm not instantiating and yet its still seems to be working?
        //I Can easily set the size of the list this way
        for (int i = 0; i < 8; i++)
        {
            squarepoints.Add(new SquarepointsPosition());

        }
        //squarepoints.Add(new Square)

    }

    // Update is called once per frame 
    void Update()
    {
        // I was going to use this to pass the input check to and then check if its true but I may not need to do it this Keycode's maybe the problem in them not working
        //bool ButtonPressed = false; 
        //This selects a position on which to search, This is left click
        if (Input.GetMouseButtonDown(0))
        {
            Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit);
            Vector2 pixelUV = hit.textureCoord;

            //Renderer rend = hit.transform.GetComponent<Renderer>();

            //Texture2D tex = rend.material.mainTexture as Texture2D;

            //This converts the 0.3423 whatever float value the pixel
            pixelUV.x *= cloony.width;
            pixelUV.y *= cloony.height;


            RootXYposition.xpoint = (int)pixelUV.x;
            RootXYposition.ypoint = (int)pixelUV.y;
            //Should be OutsidEdgeSearch

            CurrentSearch = SearchState.OutsideEdgeSearch;

            //cloony.SetPixel(RootXYposition.xpoint, RootXYposition.ypoint, Color.blue);
        }

        //This selects the colour on the texture, this is right click
        if (Input.GetMouseButtonDown(1))
        {
            Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit);
            Vector2 pixelUV = hit.textureCoord;

            pixelUV.x *= cloony.width;
            pixelUV.y *= cloony.height;

            RootXYposition.xpoint = (int)pixelUV.x;
            RootXYposition.ypoint = (int)pixelUV.y;

            roadcolor = cloony.GetPixel(RootXYposition.xpoint, RootXYposition.ypoint);

        }

        //  This is middle mouse button
        if (Input.GetMouseButtonDown(2))
        {
            Debug.Log("This is the middle mouse button");
            Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit);
            Vector2 pixelUV = hit.textureCoord;

            pixelUV.x *= cloony.width;
            pixelUV.y *= cloony.height;

            RootXYposition.xpoint = (int)pixelUV.x;
            RootXYposition.ypoint = (int)pixelUV.y;

            CurrentSearch = SearchState.NearestSearch;

            //SquareSearch(Color.blue, RootXYposition, 0.3f, SearchType.NearestColour);



        }

        //This implements the random search system, which I started with 
        //Keycodes seem to be finicky for some reason or my code is fooked in some other way
        if (Input.GetKeyDown("space"))
        {
            CurrentSearch = SearchState.RoadSearch;
            gotColor = Color.white;
            infinitetimedelay = 100.0f;


        }

        //Resets all variables and resets the texture to a clean slate
        //Need to make sure they are all in before moving on
        //For some reason this doesnt work half the time or doesnt work even at all?
        //ButtonPressed = Input.GetKeyDown("enter");keyCode.keypadEnter
        if (Input.GetKeyDown("enter"))
        {
            infinitetimedelay = 100.0f;
            gotColor = Color.white;

            Squasize = 1;


            OutsideEdgepos = false;
            EdgePos = false;
            FoundEdge = false;


            roadcolor = OriginalTexture.GetPixel(OriginalTexture.width / 2, OriginalTexture.width / 2);

            //I shouldn't need to set these as these are set at the end of functions
            //RootXYposition = 
            //EdgeXpposition =

            CurrentSearch = SearchState.DoNothing;

            textureobj.GetComponent<Renderer>().material.mainTexture = OriginalTexture;
            
            DestroyImmediate(cloony);

            DestroyImmediate(cloneCloony);
            
            cloony = Instantiate(textureobj.GetComponent<Renderer>().material.mainTexture as Texture2D);

            cloneCloony = Instantiate(textureobj.GetComponent<Renderer>().material.mainTexture as Texture2D);

            textureobj.GetComponent<Renderer>().material.mainTexture = cloneCloony;

            

        }


        while (gotColor != roadcolor && CurrentSearch == SearchState.RoadSearch)
        {
            infinitetimedelay -= Time.deltaTime;
            if (infinitetimedelay < 0)
            {
                Debug.Log("It exited using the timecounter");
                break;
            }
            //This is going to have to be redone, it just returns the colour i need the position also, I may need to do the random number generator separately and then pass it on
            //so i have it stored somewhere because it clearly wont work the way it is now


            //This clashed with the timer so do I not have the timer if I have this?
            //If I dont it might take ages to still find the road
            //If I need to test a specific thing I should probably just manually specify the position
            //if (Input.GetKeyDown("space"))
            //{//I need to change this to call and take from the actual texture so I dont have or I dont need it at all when I build the simple ui and you can manually select it or
            //Should I have it still search
            RootXYposition.xpoint = Random.Range(0, cloony.width);
            RootXYposition.ypoint = Random.Range(0, cloony.height);
            //}
            //cloony.SetPixel(150, 150, Color.black);
            gotColor = cloony.GetPixel(RootXYposition.xpoint, RootXYposition.ypoint);
            if (gotColor == roadcolor)
            {
                //I need this for the edge check
                //cloony.SetPixel(RootXYposition.xpoint, RootXYposition.ypoint, Color.blue);
                CurrentSearch = SearchState.OutsideEdgeSearch;
                break;
            }


        }

        //This is the first search
        //This part is to find the outside edge to start with, it returns or gets the result just outside the edge I then need to find the closest point inside the road This is the first search
        if (CurrentSearch == SearchState.OutsideEdgeSearch)
        {
            //0.06 seems like the max tolerance for the TestRoad5 where it seems to stay within the road I may need to put in even lower to really ensure it does and 
            //remember its nought point nought six, but I'll leave this as 0.04 for now
            FoundEdge = EdgeSearch(roadcolor, RootXYposition, 0.005f, SearchType.MostDifferent);
            if (FoundEdge)
            {
                CurrentSearch = SearchState.ActualEdgeSearch;
                Debug.Log("Its found the outside edge");
                //MyGetandSetPixelFunction(EdgeXYposition, Color.red);
                //FoundEdge = false;
                Squasize = 1;
            }
            else
            {
                Squasize++;
            }
        }

        
        //This is the second search,  This then again finds the colour that is nearest to the road colour that I want, These two search's are the only way I can figure to get the position that is nearest th edge

        if (CurrentSearch == SearchState.ActualEdgeSearch)
        {
            
            FoundEdge = EdgeSearch(roadcolor, EdgeXYposition, 0.01f, SearchType.NearestColour);
            if (FoundEdge)
            {
                //Debug.Log("It has completely found the edge");
                MyGetandSetPixelFunction(EdgeXYposition, Color.red);
             
                CurrentSearch = SearchState.DoNothing;
                Squasize = 1;
               
            }
            else
            {
                Squasize++;
            }

        }

        //
        //It finds the nearest colour and keeps going till all square are outside that range
        //It seems like this needs to be quite low, It acts a bit peculiar not sure if I understand it exactly
        if (CurrentSearch == SearchState.NearestSearch)
        {

            FoundEdge = EdgeSearch(roadcolor, RootXYposition, 0.01f, SearchType.NearestColour);
            if (FoundEdge)
            {
                Squasize++;
            }
            else
            {
                //I'm not sure if this is quite a good way of doing it, as an else I may need a more definite way to define when it has reached the limits.
                Debug.Log("It has reached the limits of the nearest colours");
                CurrentSearch = SearchState.OtherSideSearch;
                Squasize = 1;
            }

        }

        //StartCoroutine(Waitamin());

        if (CurrentSearch == SearchState.MoveAlongEdge)
        {
            textureobj.GetComponent<Renderer>().material.mainTexture = OriginalTexture;

            

            //cloony = Instantiate(textureobj.GetComponent<Renderer>().material.mainTexture as Texture2D);

            DestroyImmediate(cloony);

            DestroyImmediate(cloneCloony);

            cloony = Instantiate(textureobj.GetComponent<Renderer>().material.mainTexture as Texture2D);

            cloneCloony = Instantiate(textureobj.GetComponent<Renderer>().material.mainTexture as Texture2D);

            textureobj.GetComponent<Renderer>().material.mainTexture = cloneCloony;

            //textureobj.GetComponent<Renderer>().material.mainTexture = cloony;

            //MyGetandSetPixelFunction(EdgeXYposition, Color.red);
            //I have to be careful the way I assign variables I cant just assign it like the below its basically a reference
            //I have to dig down to the direct values if I want to assign them and not 
            //RootXYposition = EdgeXYposition;

            RootXYposition.xpoint = EdgeXYposition.xpoint;
            RootXYposition.ypoint = EdgeXYposition.ypoint;

            //EdgeXYposition.xpoint = 100;
                        
            Squasize = 1;

            //StartCoroutine(Waitamin());

            MoveAlongEdgeSuccess = MoveAlongEdge(roadcolor, RootXYposition, 5, 0.003f);
            if (MoveAlongEdgeSuccess)
            {
                Debug.Log("Hooray its moved along the edge");
              

                CurrentSearch = SearchState.DoNothing;

            }
            else
            {
                Debug.Log("Fuck it, it didn't move along the edge");
                CurrentSearch = SearchState.DoNothing;
            }


        }
                
    }

    IEnumerator Waitamin()
    {
        print(Time.time);
        yield return new WaitForSeconds(5);
        print(Time.time);
    }

    //This fooking works yay?
    bool EdgeSearch(Color SearchColor, SquarepointsPosition StartSerchPosition, float ColorDifference, SearchType CurrentType)
    {
        FoundEdge = false;
        Color squcolor;
        
        int NumSquapoints = 4;

        int ColorCounter = 0;
                    
        //Sets the squares a much more functional and easier way of doing it
        SetSquares(StartSerchPosition, false);
       
        
        //I need to compare the selected colour against the gotcolor or roadcolor.  Gotcolor is the root color, the chosen road color
        //This iterates through the squares
        for (int j = 0; j < NumSquapoints; j++)
        {
            //squcolor = cloony.GetPixel(squarepoints[j].xpoint, squarepoints[j].ypoint);
            squcolor = MyGetandSetPixelFunction(squarepoints[j], Color.blue, true);
            //CheckColor = cloony.GetPixel(squarepoints[j].xpoint, squarepoints[j].ypoint);

            //This works nice, it converts color range from 0.0 to 1.0 to the 0, 256 range
            //So I can check the values in the Color range, I need to keep them as they are as they wont work in the RGB range
            //checkthis = Mathf.Abs(squcolor[1] - SearchColor[1]);
            //checkRGB = Mathf.InverseLerp(0.0f, 1.0f, squcolor[1]);
            //resultRGB = Mathf.Lerp(0.0f, 256.0f, checkRGB);
            //originalRGB = Mathf.Abs(squcolor[1] - SearchColor[1]);
            //This checks if the searched color is an exact match to the chosen color,
            if (CurrentType == SearchType.ExactMatch)
            { 

                if (ExactMatchFunction(SearchColor, squcolor))
                {
                    EdgeXYposition.xpoint = squarepoints[j].xpoint;
                    EdgeXYposition.ypoint = squarepoints[j].ypoint;
                    
                    //FoundEdge = false;
                    break;
                }


            }
                    
            //This checks how different the colour, This is how I find edges to start with, by defining the colour difference i can define how different I want it
            //It only returns true when the distance between the road color and checked color point is greater than the colordifference.  I can change and adjust the colordifference as I wish
            if (CurrentType == SearchType.MostDifferent)
            {
                if (MostDifferentFunction(SearchColor, squcolor, ColorDifference))
                {

                    EdgeXYposition.xpoint = squarepoints[j].xpoint;
                    EdgeXYposition.ypoint = squarepoints[j].ypoint;

                    //FoundEdge = false;
                    break;
                }
                
            }
            //This keeps going until it reaches a different colour that is out of the range.
            if (CurrentType == SearchType.NearestColourMultiples)
            {
                if (NearestColourFunction(SearchColor, squcolor, ColorDifference))
                {
                    ColorCounter++;
                    Debug.Log("Why aint this shit working");
                    //I need this in here because I dont want it to break out unless its checked all the values for this function
                    FoundEdge = false;
                    
                }

            }

            if(CurrentType == SearchType.NearestColour)
            {
                if(NearestColourFunction(SearchColor, squcolor, ColorDifference))
                {
                    EdgeXYposition.xpoint = squarepoints[j].xpoint;
                    EdgeXYposition.ypoint = squarepoints[j].ypoint;


                    FoundEdge = true;
                }

            }
            //This makes sure it checks all the square before it completely returns true and doesnt just continue forever 
            if (ColorCounter == NumSquapoints)
            {
                EdgeXYposition.xpoint = squarepoints[j].xpoint;
                EdgeXYposition.ypoint = squarepoints[j].ypoint;
                
                FoundEdge = true;
                break;

            }
            
                //This means one square the first square has found to be true         
                
            
            //I dont think it requires this anymore                            
            //if (FoundEdge)
            {
                //EdgeXYposition.xpoint = squarepoints[j].xpoint;
                //EdgeXYposition.ypoint = squarepoints[j].ypoint;
                //MyGetandSetPixelFunction(squarepoints[j], Color.magenta);
                // break;
            }

        }

        //I use these to do the final edge check it checks for this colour when it checks the final edge
        //Do I need this here though, this feels out of place in the wrong place if it just applies to one search type then its not really needed here I can just place it near to that
        if (CurrentSearch == SearchState.OutsideEdgeSearch)// || CurrentSearch == SearchState.OtherSideSearch)
        {
            for (int i = 0; i < NumSquapoints; i++)
            {
                //MyGetandSetPixelFunction(squarepoints[i], Color.blue, false);
                //cloony.SetPixel(squarepoints[i].xpoint, squarepoints[i].ypoint, Color.blue);
                //cloony.Apply();
            }
        }


        //I need this to return the resultant edgepoints namely EdgeXYposition, do i, I dont know, like if its a public variable
        return (FoundEdge);
    }

    //Just putting these here as a reminder
    //public SquarepointsPosition StartEdgePosition;
    //public SquarepointsPosition EndEdgePosition;
    //public SquarepointsPosition MiddleEdgePosition;
    //public SquarepointsPosition CurrentEdgePosition;


    bool MoveAlongEdge(Color SearchColor, SquarepointsPosition OriginalPosition, int Howfar, float ColorDiff)
    {
        int MaxMove = 5;
        int MinMove = 2;

        int CurrIternumb = 0;

        Color Squcolor = Color.white;

        infinitetimedelay = 500.0f;

        int EdgeCounter = 0;

        OutsideEdgepos = false;
        EdgePos = false;
        FoundEdge = false;


        //Squasize = 1;

        //int EdgeCounter = 0;

        //I'm rubbish with variable names, shrug
        //I need this variable to only return true when its found 
        //bool MoveAlongEdgeSlight = false;
        //bool NearestColorTrue = false;

        //bool MostDifferentTrue = false;
        
        //enum SquareMode { FirstEdgePoint, SecondEdgePoint};        
        //I need to assign it at the same time
        //SquarepointsPosition BackupEdgePosition = StartSerchPosition;
        //I'm just setting these up to this as a start thing
        StartEdgePosition = OriginalPosition;
        SquarepointsPosition PotentialEdgePos = OriginalPosition;
        SquarepointsPosition ConfirmedEdgePos = OriginalPosition;
        SquarepointsPosition PreviousEdgePos = OriginalPosition;
        SquarepointsPosition temppos = OriginalPosition;

        //Assigns the squares to all 8 positions
        //SetSquares(StartSerchPosition, true);

        //This is iterating to move along the edge not sure about this if its a good place to have it
        //This is tho old function I'm going to start again
        //while (EdgeCounter < MaxMove)
        {
            infinitetimedelay -= Time.deltaTime;
            if (infinitetimedelay < 0)
            {
                Debug.Log("It exited using the timecounter");
                //break;
            }
            //This is iterating to move around the squares
            //I'm pretty sure I need to set these here
            //I need to backup the position and then go back to it if it the most different function isnt true
            //BackupEdgePosition = 
            
            SetSquares(ConfirmedEdgePos, true);
            for (int i = 0; i < 8; i++)
            {
                //Squcolor = cloony.GetPixel(squarepoints[i].xpoint, squarepoints[i].ypoint);
                Squcolor = MyGetandSetPixelFunction(squarepoints[i], Color.blue, true);
                //MyGetandSetPixelFunction(squarepoints[i], Color.red);
                if (NearestColourFunction(SearchColor, Squcolor, 0.05f))
                {
                    NearestColorTrue = true;
                    //MyGetandSetPixelFunction(squarepoints[i], Color.red);
                    //PotentialEdgePos = squarepoints[i];
                    PotentialEdgePos.xpoint = squarepoints[i].xpoint;
                    PotentialEdgePos.ypoint = squarepoints[i].ypoint;
                    //Somehow this fucking Setsquares function is changing and moving the PotentialEdgePos how the fuck is it even doing that
                    //Its not even supposed to fucking even be able to do that 
                    temppos.xpoint = squarepoints[i].xpoint;
                    temppos.ypoint = squarepoints[i].ypoint;

                    //This doesnt work
                    //temppos = squarepoints[i];


                    SetSquares(temppos, true);
                    //PotentialEdgePos = squarepoints[i];
                    for (int k = 0; k < 8; k++)
                    {
                        //SetSquares(PotentialEdgePos, true);
                        //Squcolor = cloony.GetPixel(squarepoints[k].xpoint, squarepoints[k].ypoint);
                        Squcolor = MyGetandSetPixelFunction(squarepoints[k], Color.blue, true);
                        if (MostDifferentFunction(roadcolor, Squcolor, 0.06f))
                        {
                            MoveAlongEdgeSlight = true;
                            //MyGetandSetPixelFunction(squarepoints[i], Color.red);
                            //cloony.SetPixel(squarepoints[k].xpoint, squarepoints[k].ypoint, Color.yellow);
                            //cloony.Apply();
                            //CheckColor = cloony.GetPixel(squarepoints[k].xpoint, squarepoints[k].ypoint);
                            //ConfirmedEdgePos = PotentialEdgePos;
                            ConfirmedEdgePos.xpoint = PotentialEdgePos.xpoint;
                            ConfirmedEdgePos.ypoint = PotentialEdgePos.ypoint;
                            CheckCounter++;
                            EdgeCounter++;
                            

                            break;
                            
                        }
                        else if(k == 7 && MoveAlongEdgeSlight == false)
                        {
                            //I think its going into this loads of times and is so overloading the debug log maybe put a counter in it to see how many times
                            Debug.Log("So it went into this function?");
                            SetSquares(ConfirmedEdgePos, true);

                        }
                      

                    }

                    if (EdgeCounter == 1)
                    {

                        //SecondEdgePosition = ConfirmedEdgePos;
                        SecondEdgePosition.xpoint = ConfirmedEdgePos.xpoint;
                        SecondEdgePosition.ypoint = ConfirmedEdgePos.ypoint;
                    }
                    else if (EdgeCounter == 4)
                    {
                        //EndEdgePosition = ConfirmedEdgePos;
                        EndEdgePosition.xpoint = ConfirmedEdgePos.xpoint;
                        EndEdgePosition.ypoint = ConfirmedEdgePos.ypoint;
                        MoveAlongEdgeSuccess = true;
                        //It seems like I definitely need break here its getting closer to the edge with this but its still not actually on the edge for some weird reason
                        break;

                    }



                }

                if(MoveAlongEdgeSlight)
                {
                    //NearestColorTrue = false;
                    //MoveAlongEdgeSlight = false;
                    break;
                }
                        
                                               
            }
                                     
        }
                
        MyGetandSetPixelFunction(StartEdgePosition, Color.red);
        MyGetandSetPixelFunction(SecondEdgePosition, Color.red);
        //MyGetandSetPixelFunction(EndEdgePosition, Color.red);
        
        return (MoveAlongEdgeSuccess);

    }

    void SetSquares(SquarepointsPosition StartSerchPos, bool NumofSquares)
    {
        //I do need to error check this make sure all values are applied and valid and arent out of range or whatever likely errors can occur
        squarepoints[0].xpoint = StartSerchPos.xpoint + Squasize;
        squarepoints[0].ypoint = StartSerchPos.ypoint - Squasize;
        //What the fuck is this shit is it a bug?  The two lines below overwrite the actual StartSerchPos variable which then overwrites the PotentialEdgePos variable
        squarepoints[1].xpoint = StartSerchPos.xpoint - Squasize;
        squarepoints[1].ypoint = StartSerchPos.ypoint - Squasize;

        squarepoints[2].xpoint = StartSerchPos.xpoint - Squasize;
        squarepoints[2].ypoint = StartSerchPos.ypoint + Squasize;

        squarepoints[3].xpoint = StartSerchPos.xpoint + Squasize;
        squarepoints[3].ypoint = StartSerchPos.ypoint + Squasize;
        //Only use these if needed
        if (NumofSquares == true)
        {
            squarepoints[4].xpoint = StartSerchPos.xpoint;
            squarepoints[4].ypoint = StartSerchPos.ypoint - Squasize;

            squarepoints[5].xpoint = StartSerchPos.xpoint + Squasize;
            squarepoints[5].ypoint = StartSerchPos.ypoint;

            squarepoints[6].xpoint = StartSerchPos.xpoint;
            squarepoints[6].ypoint = StartSerchPos.ypoint + Squasize;

            squarepoints[7].xpoint = StartSerchPos.xpoint - Squasize;
            squarepoints[7].ypoint = StartSerchPos.ypoint;
        }
        //This does not work as a buckup position it will assign it straight when it assigns the first one
        //SquarepointsPosition BackupEdgePosition = StartSerchPos;

    }

    //Checks that the colours match exactly, silly me I have to make sure that all the 3 colours match for this to be useful, even sillier me it is comparing the aggregate colors directly already
    bool ExactMatchFunction(Color SearchColor, Color CurrColor)
    {
        FoundEdge = false;
        if (CurrColor == SearchColor)
        {

            FoundEdge = true;
            //EdgeXYposition.xpoint = SearchPosition.xpoint;
            //EdgeXYposition.ypoint = SearchPosition.ypoint;

            //break;
        }
    
               
        return (FoundEdge);
    }

    //Returns true as soon as the colors are beyond a certain distance, So this is best for finding different colours to however different I want them.
    bool MostDifferentFunction(Color SearchColor, Color CurrColor, float ColorDiff)
    {
        FoundEdge = false;

        for (int i = 0; i < 3; i++)
        {
            if (Mathf.Abs(CurrColor[i] - SearchColor[i]) > ColorDiff)
            {
                FoundEdge = true;
                //EdgeXYposition.xpoint = SearchPosition.xpoint;
                //EdgeXYposition.ypoint = SearchPosition.ypoint;

                //With this I want to break as soon as at least one of the three color values is out of range
                break;
            }
        }

        return (FoundEdge);
    }

    //With this I want to check that all 3 color variables are true instead of just one otherwise it always(mostly always returns true) as its only checking one value
    //This is not working for some reason
    bool NearestColourFunction(Color SearchColor, Color CurrColor, float ColorDiff)
    {
        FoundEdge = false;
        int ColourDude = 0;

        for(int i = 0; i < 3; i++)
        {
            if(Mathf.Abs(CurrColor[i] - SearchColor[i]) < ColorDiff)
            {
                ColourDude++;
                
            }

        }

        if(ColourDude == 3)
        {
            FoundEdge = true;
            //EdgeXYposition.xpoint = SearchPosition.xpoint;
            //EdgeXYposition.ypoint = SearchPosition.ypoint;
        }

        return (FoundEdge);
    }

    //This function will serve as the get and set it will work as a debug system thing as well I can send these as a result to another texture quite easily?  So that I can see what is going on
    //without interuppting the flow of the program that much. I could even have the texture source changeable and set as a 
    //Holy smoke I think it fricking works, I think I may need to just be able to set pixels on the fake texture
    Color MyGetandSetPixelFunction(SquarepointsPosition Pos, Color SetColor, bool Getistrue)
    {
        //bool GetSetTrue = false;
        Color GotColor = Color.white;

        if (Getistrue)
        {
            GotColor = cloony.GetPixel(Pos.xpoint, Pos.ypoint);

        }
        else if (!Getistrue)
        {
            //Only use this if I absolutely have to to make it easier setting pixels can sometimes make it easier to go back and find places I've been, will probably need this to find
            //the other side of th road
            cloony.SetPixel(Pos.xpoint, Pos.ypoint, SetColor);
        }

        gotColor = GotColor;

        //Have to be careful with this I cant set the CurrColor to the getpixel because it is likely the same color so its just drawing the same color so I wont be able to see it
        //I need to write a function that checks the pixels that are overwritten, I'll have a special colour that I dont want overwritten so I can see certain specific things that I choose in all the chaos
        //So the function will need to be in this one and the one below or I just have it a seperate function on its own but it first only allows the pixels to be set as long as they are anything but the special colour
        if (cloneCloony.GetPixel(Pos.xpoint, Pos.ypoint) == Color.red)
        {
            Debug.Log("It found the red color");
            cloneCloony.SetPixel(Pos.xpoint, Pos.ypoint, Color.yellow);
            cloneCloony.Apply();

        }
        else
        {
            cloneCloony.SetPixel(Pos.xpoint, Pos.ypoint, SetColor);
            cloneCloony.Apply();
            //cloony.Apply();
        }

        
        return (GotColor);
    }

    //This is just the non overloaded function which I can use to set pixels, so draw on the viewable texture, Red is the important colour that I dont want to overwrite
    Color MyGetandSetPixelFunction(SquarepointsPosition Pos, Color SetColor)
    {
        //Red is the important colour which is not to be overwritten so I can see specific details in all chaos when I need to
        if (cloneCloony.GetPixel(Pos.xpoint, Pos.ypoint) == Color.red)
        {
            Debug.Log("It found the red color");
            cloneCloony.SetPixel(Pos.xpoint, Pos.ypoint, Color.yellow);
            cloneCloony.Apply();
        }
        else
        {
            cloneCloony.SetPixel(Pos.xpoint, Pos.ypoint, SetColor);
            cloneCloony.Apply();
        }

        return (SetColor);
    }
      

}




