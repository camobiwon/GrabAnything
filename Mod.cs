using MelonLoader;
using ModThatIsNotMod;
using StressLevelZero.Interaction;
using UnityEngine;

namespace Camo {
	public class Mod : MelonMod {
		private Transform rig;
		private bool modToggle;

		public override void OnApplicationStart() {
			InitialRegisters();
		}

		public override void OnSceneWasInitialized(int buildIndex, string sceneName) {
			rig = Player.GetRigManager().transform; //Get player rig

			if(modToggle)
				SetAllGrips(); //Set all items to grippable
		}

		public override void OnUpdate() {
			//Manually rerun grip setup
			if(Input.GetKeyDown(KeyCode.O)) {
				SetAllGrips();
			}
		}

		private void SetAllGrips() {
			Collider[] allCol = Resources.FindObjectsOfTypeAll<Collider>(); //Find all colliders

			for(int i = 0; i < allCol.Length; i++) { //Loop through all colliders
				if(!allCol[i].isTrigger && (allCol[i].GetComponent<Interactable>() == null || allCol[i].GetComponent<InteractableHost>() == null) && allCol[i].GetComponent<Grip>() == null && !allCol[i].transform.root.gameObject.name.Contains("NimbusGun") && !allCol[i].transform.root.gameObject.name.Contains("DEVMANIPTOOL") && !allCol[i].transform.root.gameObject.name.Contains("BoardGun")) { //Make sure collider does not already have a grip or is Nimbus / Dev Manip / Board Gun (They break for some reason)

					if(allCol[i].transform.root != rig && allCol[i].gameObject.layer != 5 && allCol[i].gameObject.layer != 18 && allCol[i].gameObject.layer != 19 && allCol[i].gameObject.layer != 23 && allCol[i].gameObject.layer != 25 && allCol[i].gameObject.layer != 27) { //Make sure collider is on a standard layer
						allCol[i].gameObject.layer = 15; //Set to "Interactable" layer
					}

					//Setup generic grip
					GenericGrip grip = allCol[i].gameObject.AddComponent<GenericGrip>();

					grip.type = Grip.Type.SECONDARY;
					grip.isThrowable = true;
					grip.gripOptions = InteractionOptions.MultipleHands;
					grip.priority = 4;
					grip.bodyDominance = 1;
					grip.minBreakForce = float.PositiveInfinity;
					grip.maxBreakForce = float.PositiveInfinity;
					grip.defaultGripDistance = float.PositiveInfinity;
					grip.gripDistanceSqr = float.PositiveInfinity;

					//Setup Interactable / InteractableHost
					allCol[i].gameObject.AddComponent<Interactable>().defaulGripScore = grip.minBreakForce = float.PositiveInfinity;
					InteractableHost iH = allCol[i].gameObject.AddComponent<InteractableHost>();
					iH.isStatic = true;
				}
			}
		}

		private void InitialRegisters() {
			//ModPrefs
			MelonPreferences.CreateCategory("GrabAnything");
			MelonPreferences.CreateEntry("GrabAnything", "ModToggle", true);

			//Cached vars
			modToggle = MelonPreferences.GetEntryValue<bool>("GrabAnything", "ModToggle");

			//MTINM menu settings
			ModThatIsNotMod.BoneMenu.MenuCategory category = ModThatIsNotMod.BoneMenu.MenuManager.CreateCategory("Grab Anything", Color.green);
			category.CreateFunctionElement("CHANGES REQUIRE SCENE RELOAD", Color.red, null);
			category.CreateBoolElement("Mod Toggle", Color.white, modToggle, ToggleMod);
		}

		void ToggleMod(bool toggle) {
			modToggle = !modToggle;
			MelonPreferences.SetEntryValue<bool>("GrabAnything", "ModToggle", modToggle);
		}
	}
}