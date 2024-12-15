# Class 4

In this third class, you'll learn the following:
- Adding sound effects / music
- Importing unity packages
- Using Unity's AI Navigation system
- Spawning GameObjects
- Creating a custom shader

## Adding sound effects

We'll start off quite easily, since all you have to do for now is to import 
[this chest sound effect](ClassTutorialAssets/chest_opening.wav).
Once you're done, go to your chest prefab and add a `AudioSource`component. 
Assign the audio clip, untick the play on awake button and you're almost done. 

There are quite a lot of parameters to customize your sound, 
however there is no easy way to test the AudioSource in the editor 
*(mostly because it needs an AudioListener to spatialize the sound and 
using one on the editor camera could be very confusing when you don't know 
what's happening, but you can still write your own editor script to allow 
such a behaviour)*, so if you want to try out different settings, 
you could simply tick the PlayOnAwake and loop options before entering
play mode and then switching test the values. 

For now, just changing the pitch to 0.6 or leaving it as is should be enough
to get a decent sfx.

Now, the only thing left is to tell our chest to play the sound effect when opened.

```cs
// ...
AudioSource sfx;

private void Start()
{
	// ...
	sfx = GetComponent<AudioSource>();
}

public void Interact()
{
	// ...

	sfx.Play();
}

```

## Adding a slime

If you have correctly followed the previous classes, 
this part should not give you any trouble. 

Import the [Slime](ClassTutorialAssets/Class4/lp_slime.fbx) model 
and give him a collider (and a color if you want to). 
Add an animator controller to play the Idle animation. 
Don't forget to check the `Loop Time` import setting
in the model animation tab for the Idle animation, 
otherwise it will only play once.

![slime](ClassTutorialAssets/Class4/img/slime.png)


## The AI Navigation package

Now that we have an entity in our game, we will be adding it a 
pathfinding algorithm that will make it move wherever we want. 
Using such a tool will allow us to define different areas 
(like walkable, not walkable, swim area, etc) with different behaviours
and handle all interactions between moving entities automatically.

For that, we will need to import the Unity AI Navigation package.

### Importing the package

Go to Window->Package Manager. The first page shows you which packages 
are already installed in your project. You might already have the AI Navigation 
package installed. In that case, just check that you have the latest version and 
you're good to go. Otherwise, go to the Unity Registry tab and enter 'AI' 
in the search bar. You should see it appear. Click install.

![navigation package](ClassTutorialAssets/Class4/img/navigation_package.png)

### Creating the navmesh

Select your Environment GameObject, and add a `NavMeshSurface` component. 
This is where you will define the areas for the pathfinding. 
But before we create the navmesh, we will create a new agent type, 
since our slime is not `humanoid`. Click on Agent Type, and open the 
Navigation settings (or Window->AI->Navigation). 

Create a Slime agent, and set its values. I'll go for a radius and height of 
0.75, a slope of 50° and a step height of 0.6, but it's up to you to choose values that fit the way you want your 
characters to behave, so testing is often in order.

![slime agent type](ClassTutorialAssets/Class4/img/slime_agent_type.png)

Now, go back to your NavMeshSurface component.

Change the agent type, and under Object collection, 
select current object hierarchy to only use the objects children of the 
current one (our environment in that case). You can also specify which 
layers you want to include, in our case only the default and interactable ones.

Then, hit bake and you should see a lot of blue polygons appear on your scene.

![first navmesh](ClassTutorialAssets/Class4/img/first_navmesh.png)

If you take a closer look, you'll see that what's in blue defines the area 
(walkable in our case) we juste baked, and that there are holes around the trees.
This is almost perfect. The problem we have is that our chest was not taken into 
account when baking, even if it is inside the current hierarchy.

This comes from the fact that we baked using `Render meshes` as the `Use geometry`
option, and skinned meshes (what we have for animated meshes) are not compatible 
with this. Instead, you could use the `Physics colliders` option, but other problem
could arise. An alternative solution is adding the `NavMeshObstacle` component 
to the problematic object, which will let us define a custom bounding box which
the navmesh will process at runtime. Note that this method is usually 
used for moving objects.

Add one to your chest prefab, and select the `Carve` option. 
Adjust the size to your needs, you should see the surface update in realtime.

### Making an agent

Now that we've made an approximately correct navmesh, let's add agents thats will 
use it. On your slime prefab, add a `NavMeshAgent` component and set its type to slime.
Then, adjust the settings to your needs. I go with the following:

![agent settings](ClassTutorialAssets/Class4/img/agent_settings.png)

The next step is making a script that will set a destination for the agent. 
For starters, we'll test by simply giving the player as a destination so that 
the agent will always follow them.

```cs

using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SimpleSlime : MonoBehaviour
{
    NavMeshAgent agent;
    
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (PlayerController.Instance != null)
            agent.SetDestination(PlayerController.Instance.transform.position);
    }
}

```

The `RequireComponent` directive will force the presence of a specified component 
on the object the script is placed.

Running the game now should make the slime follow you and avoid the trees and obstacles.

#### Optimisation concerns

As it is, the scene works quite nicely, but you can very quickly run into 
performance issues. If you want to see them appear, just add a few slimes.
Depending on your computer, it might take from a few slimes to hundreds of them.
Of course, adding more entities will also cause other perfomance issues such has 
a very high polygon count, etc. However, some of those issues might come from the
AI Navigation. 

The first thing to consider is how we set the destination of our agents. Right now, 
we are changing it every frame, so every agent recalculates its path every frame.
If we have 10 agents running at 60 fps, that's already 600 path computations
every second. For basic computation like addition, this poses absolutely no problem.
However, the algorithm can be quite performance heavy depending on the complexity of 
the navmesh. And in fact, the player might not even notice if the agent only recalculates 
its path once every second or so, unless moving at very high speed.

Adding a timer would be a first optimisation. You could also use a `Coroutine`, 
which we'll discuss in a later part of the class. In fact, making a lot of 
unique calls to a method (especially `Update`) for the same type of logic is pretty 
inefficient. To improve it even more, you could create a general script for handling 
all slimes in a single call. And if you want to go even further, 
you might want to create some sort of LOD (Level Of Detail) that will scale the 
rate of computation depending on the distance to the player, meaning that objects 
very far will be update way less often.

Finally, another thing to consider is the obstacle avoidance `Quality` setting.
Changing this affects how precisely the agent tries to avoid other agents and 
obstacles, and can easily improve the performance.

## Spawning the slimes

If you have ever played a survival/rpg game, you know that monsters need to spawn 
out of thin air. Either their placement is predefined and they respawn when you die
or reload the area, or they spawn continuously around a center point.

Because we love infinite experience points farming, we'll make a continuous spawner.

Let's also make it a bit generic so that we can reuse it to spawn other entities.
reate a new C# Script, `EntitySpawner`.

We'll be needing a few different values : 

```cs
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EntitySpawner : MonoBehaviour
{
	public GameObject entityPrefab;
	public Transform entitesParent;
    public float spawnRate = .1f;

    public float spawnRadius = 5;
	private float samplingDistance = 1f;

	public float maxQuantity = 10f;
}
```

### Coroutines

In order to spawn our entities at a constant rate, we could create a timer that would 
get incremented every frame by `Time.deltaTime`. However, we'll be using a more appropriate concept, `Coroutines`.
Coroutines are asynchronous methods that can wait until a certain condition is satisfied before 
processing its code, without blocking the main loop.

```cs

private void Start()
{
    StartCoroutine(SpawnEntity());
}

private IEnumerator SpawnEntity()
{
    while (true) 
    {
        yield return new WaitForSeconds(1 / spawnRate);
        Debug.Log("spawning");
    }
}
```

This coroutine will print the "spawning" message indefinitely once every ten seconds (for a spawn rate of 0.1f).
You can also return `null` if you want to wait until the next frame update.

### Finding a valid spawn point

Since our entities are using the navmesh, we need to spawn them on it otherwise we'll get some errors.
We could spawn them at the position of the spawner, but we might prefer to spawn them randomly in a certain
radius around the spawner. 

Our terrain is flat, so we'll just find a random point in a circle and try to find the closest possible 
position on the navmesh. However, if your terrain had elevation changes, you'd need to account for that when 
sampling the position on the navmesh. A look at the [documentation](https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html)
for the method we'll be using will tell you more about the problems you might encounter.

```cs

private IEnumerator SpawnEntity()
{
	NavMeshHit hit;
	Vector3 spawnPos;
	while (true)
	{
		yield return new WaitForSeconds(1 / spawnRate);
		if (entitesParent.childCount >= maxQuantity) continue;

		spawnPos = Random.insideUnitCircle * spawnRadius;
		// insideUnitCircle returns a vector2, so we need to switch some values
		spawnPos.z = spawnPos.y;
		spawnPos.y = 0;

		spawnPos += transform.position;
		
		if (NavMesh.SamplePosition(spawnPos, out hit, samplingDistance, NavMesh.AllAreas))
			spawnPos = hit.position;
		else spawnPos = transform.position;
	}
}

```

### Instantiating

The spawner is almost done. We only need to do the actual spawning of the entity.
We'll call the `Instantiate()` method from the GameObject class. This will create a copy of the 
given gameobject and add it to the scene at runtime.

```cs
private IEnumerator SpawnEntity()
{
	// ...
	while (true)
	{
		// ...
		Instantiate(entityPrefab, spawnPos, Quaternion.identity, entitesParent);
	}
]
```

We also specify where to spawn the object, with what orientation and as a children of which object.
If you want further control on the spawned object, the method returns its reference.


If you want, you can set a custom icon for your script. Select it, and click 
on the arrow under the default icon.

![agent settings](ClassTutorialAssets/Class4/img/script_icon.png)

Now, you can create a slime spawner object and a slime parent object (it will hold all the instances, so it's
better if it is a stationary object with its transform set to default values). Assing the values to the script,
and try it.

![slime wave](ClassTutorialAssets/Class4/img/slime_wave.png)

## Creating a custom shader

If you've come this far, well done! You can be proud of yourself.

However, before we say goodbye, let's do one final thing. Unity comes with a very powerful tool when it comes 
to making shaders, the code behind the materials we used to color our objects. This tool is called `ShaderGraph`.

This will allow you to create your own shaders without having to learn any shader language, by linking logic 
gates together. This simplifies and renders the process accessible to almost anyone, as long as they do not fear
doing a little bit of maths.

Today, we'll be making a shader for our slime, giving it a more transparent and vibrant look.

### Shader graph

In your project window, right-click Create->Shader Graph->URP->Lit Shader Graph and name it SlimeSahder.
Double click to open it in the shader graph window.

You'll see two main nodes in your blackboard, `vertex` and `fragment`. Vertex will be used to move the 
object geometry, for example for making water waves, while fragment is where we'll work with the pixel's colors.
At the left, you can find the properties of your shaders that will be available on the material, like the 
color field we changed on the default materials. At the top right, you'll find the graph inspector. 

Since we said we'd make our slime transparent, we'll need to change the `surface type` from `Opaque` 
to `Transparent` in the graph inspector. you can also set the Preview window to Custom Mesh->Slime body to 
see the slime instead of a sphere (if the preview doesn't show up, try closing and reopening the window).

The first thing we'll be doing is creating a new `BaseColor` property from the "+" icon,
setting its default value and then linking it to the Base Color in the fragment node 
(drag and drop the property to add it to the blackboard).

![shader base color](ClassTutorialAssets/Class4/img/shader_base_color.png)

Then, press space to add a new node. Search for the `Fresnel Effect` one.
This node generates white when near the borders of the mesh and black in the center. 
We will then add another color property, but this time call it GlowColor and set its mode to `hdr`.
HDR color nodes will be read by the post processing volumes to apply bloom in the scene. 
We'll get to that part later.
Add a multiply node to connect the output of the fresnel and the glow color, and link it 
to the emission field.
Finally, create a float property to control the fresnel strength, and another one for the alpha channel
(you can set its mode to slider with values between 0 and 1).

Your graph should look something like this:

![final graph](ClassTutorialAssets/Class4/img/final_graph.png)


Now, go back to your project window, and right-click on your shader, create material. 
This will create a material with the correct shader already selected.
Assign it to your slime prefab, and adjust the values as you see fit.

It should look like this:
![slime with shader](ClassTutorialAssets/Class4/img/slime_with_shader.png)

If you cannot see the gloaw around the slime, there are a few reasons why that might be:
- the intensity value for the glow color is too small / the color is too dark
- the post-processing checkbox on the main camera is not set to true
- you do not have a default volume with bloom.

### Post processing and graphics settings

But what is a post processing volume? Its a component that will define a zone where 
the selected post processing effets are applied to the camera, such as bloom, vignette, 
color correction, tonemapping, etc.

If you go to the settings folder in your assets, you should find a file called `Sample Scene Profile` 
and another one called `Default Volume`. If you select the `PC_RPAsset` file 
(this is the main asset that defines the quality/graphics options for your game), you'll see under the `Volumes`
tab that either of the two mentionned is selected. You can try changing a few of the volume file settings 
like bloom or vignette, and see how that affects your game view. 

If you want to set a specific volume area or change the default volume for only this scene, juste create 
a new gameobject, add a volume component, and create a new volume asset.

![having fun with volumes](ClassTutorialAssets/Class4/img/having_fun_with_volumes.png)

## Conclusion

*Et voilà!* You're done with this course. It's now up to you if you want to expand on 
what we've built so far, or if you want to start your own project. 

However, remember this was just an introduction, as we've only scratched the surface of 
what the engine has to offer. 

I hope you have fun in your gamedev journey, and I'd love to see what you'll make in the future!

Goodbye!

---
*course by Julien Charvet for GCC*

[previous class](https://github.com/Skydrag42/GCC_Unity_Forma_Class3/)