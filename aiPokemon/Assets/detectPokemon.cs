using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class detectPokemon : MonoBehaviour
{
    RaycastHit hit;
    Ray ray;
    
    int layerPokemon;
    float detectionDistance = 6.0f;

    string prevPokemon;
    
    myClient client;

    Text pokedexText;

    // Start is called before the first frame update
    void Start()
    {
        layerPokemon = LayerMask.GetMask("Pokemon"); // Ignore other layers than Pokemon-layer
        client = GameObject.Find("unityClient").GetComponent<myClient>(); // Find ServerClient script
        pokedexText = GameObject.Find("Pokedex").transform.Find("Pokemon").GetComponent<Text>(); // Pokedex screen text
    }

    // Update is called once per frame
    void Update()
    {
        if (client.isConnected)
        {
            if (client.pokemonSearching)
            {
                Debug.Log("Searching for Pokemons!");

                ray = new Ray(transform.position, transform.forward);

                if (Physics.Raycast(ray, out hit, detectionDistance, layerPokemon))
                {
                    string pokemon = hit.transform.gameObject.GetComponent<RawImage>().texture.name;

                    if (prevPokemon != pokemon) // Don't wanna classify what we just scanned before
                    {
                        Debug.Log("A wild Pokémon appeared!");
                        pokedexText.text = "scanning ...";
                        client.SendMessage(pokemon);

                        client.pokemonSearching = false;
                        prevPokemon = pokemon;
                    }
                }
            }
        }
    }
}
