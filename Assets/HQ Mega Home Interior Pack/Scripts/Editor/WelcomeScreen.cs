using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using util = WelcomeScreenUtility;

namespace WatchFindDoMedia
{
    class Prefs
	{
		public static string LaunchKey => "WelcomeScreen_Initialized_State_" + Application.productName;
	}

	[InitializeOnLoad]
	class WelcomeScreenLoader
	{
		static WelcomeScreenLoader()
		{
			var state = EditorPrefs.GetBool("WFDM_DoNotShowAtStartWelcomeWindow");
			if (!state)
            {
				EditorApplication.update += Update;
			}
		}

		static void Update()
		{
			EditorApplication.update -= Update;

			if (!EditorApplication.isPlayingOrWillChangePlaymode)
			{
				bool show = false;
				if (!EditorPrefs.HasKey(Prefs.LaunchKey))
				{
					show = true;
					EditorPrefs.SetBool(Prefs.LaunchKey, true);
				}
				else
				{
					if (Time.realtimeSinceStartup < 10)
					{
						show = EditorPrefs.GetBool(Prefs.LaunchKey, true);
					}
				}

				if (show)
				{ WelcomeScreen.Open(); }
			}
			if (!EditorPrefs.GetBool("WelcomeScreen_Initialized_State"))
			{ EditorWindow.GetWindow<WelcomeScreen>(); }
		}
	}

	class UIButton
    {
		public readonly VisualElement element;
		public readonly VisualElement view;

		public bool Hovered  { get; private set; }
		public bool Selected { get; private set; }

		public Action OnSelected = delegate { };

		public UIButton(VisualElement element, VisualElement view)
        {
            this.element = element; this.view = view;
			RegisterEvents();

			Selected = element.ClassListContains("active");

            switch (Selected)
            {
                case true:
					Select();
					break;
				case false:
					Deselect();
					break;
            }
        }

		public void RegisterEvents()
        {
			Selected = element.ClassListContains("active");

			element.RegisterCallback<MouseEnterEvent>((_) => {
				if (!element.ClassListContains("hover") && !element.ClassListContains("active"))
				{ element.AddToClassList("hover"); }

				Hovered = true;
			});
			element.RegisterCallback<MouseLeaveEvent>((_) => {
				if (element.ClassListContains("hover"))
				{ element.RemoveFromClassList("hover"); }

				Hovered = false;
			});
			element.RegisterCallback<MouseDownEvent>((_) => {
				if (!element.ClassListContains("pressed"))
				{ element.AddToClassList("pressed"); }
			});
			element.RegisterCallback<MouseUpEvent>((_) => {
				if (element.ClassListContains("pressed"))
				{ element.RemoveFromClassList("pressed"); }

				if (!Selected)
                {
					Select(); OnSelected();
				}
			});
		}

		public void Select()
        {
			if (!element.ClassListContains("active"))
			{ element.AddToClassList("active"); }

			view.style.display = DisplayStyle.Flex;

			Selected = true;
		}
		public void Deselect()
        {
			if (element.ClassListContains("active"))
			{ element.RemoveFromClassList("active"); }

			view.style.display = DisplayStyle.None;

			Selected = false;
		}
    }

	public class WelcomeScreen : EditorWindow
	{
		const string headerImageURL = "https://assetstorev1-prd-cdn.unity3d.com/package-screenshot/5058a203-3c92-4812-ba53-b20f80cf3049_scaled.jpg";
        
		static readonly Dictionary<string, string> resources = new Dictionary<string, string>
		{
			{ "ico-pdf", "https://img.icons8.com/color/48/000000/pdf.png" },
			{ "ico-assetstore", "https://img.icons8.com/ios-filled/50/000000/unity.png" },
			{ "ico-youtube", "https://img.icons8.com/color/48/000000/youtube-play.png" },
			{ "ico-github", "https://img.icons8.com/material-outlined/24/000000/github.png" },
			{ "ico-discord", "https://img.icons8.com/color/48/000000/discord-logo.png" },
		};

		bool noInternet = false;

		[SerializeField] VisualTreeAsset visualTree;
		[SerializeField] StyleSheet style;
		[SerializeField] UnityEngine.Object manual;
		[SerializeField] UnityEngine.Object hdrpUpgrade;
		[SerializeField] UnityEngine.Object urpUpgrade;

        readonly List<UIButton> buttons = new List<UIButton>();

		[MenuItem("Window/WFDM/Welcome", false, 18)]
		public static void Open()
		{
			var window = EditorWindow.GetWindow<WelcomeScreen>(true, "Welcome");
			window.minSize = new Vector2(650, 500);
			window.maxSize = new Vector2(650, 500);
			
			window.Show(); 
		}

		void OnEnable()
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
            {
				noInternet = true;
            }

			if (noInternet) { return; }

			this.SetAntiAliasing(4);

			rootVisualElement.styleSheets.Add(style);
			rootVisualElement.Add(visualTree.CloneTree());

			DownloadHandlerTexture texHandler = new DownloadHandlerTexture(true);

			EditorCoroutineRunner.StartCoroutine(util.Request(headerImageURL, texHandler, (DownloadHandler handler) =>
			{
				var headerTexture = ((DownloadHandlerTexture)handler).texture;

				var header = rootVisualElement.Q<Image>("", "inner-image");
				header.image = headerTexture;

			}));

			#region Text Config
            var text = rootVisualElement.Query<TextElement>().Build().ToList();

            for (int i = 0; i < text.Count; i++)
            {
				if (!text[i].text.Contains("`")) { continue; }

				var splitInTabs = text[i].text.Split('`');
				
				StringBuilder rawText = new StringBuilder();
				for (int s = 0; s < splitInTabs.Length; s++)
				{
					rawText.AppendLine(splitInTabs[s]);
				}
				text[i].text = rawText.ToString();
			}
			#endregion

			EditorCoroutineRunner.StartCoroutine(LoadResources());

			rootVisualElement.Query<VisualElement>("", "social-content").Build().ToList()[0].style.marginTop = 0;

			var socialUrls = rootVisualElement.Query<TextField>("", "readonly").Build().ToList();
            for (int i = 0; i < socialUrls.Count; i++)
            {
				socialUrls[i].isReadOnly = true;
            }

			var detailsBtn = rootVisualElement.Q("nav-btn-details", "nav-button");
			buttons.Add(new UIButton(detailsBtn, rootVisualElement.Q("details-holder")));

			var packagesBtn = rootVisualElement.Q("nav-btn-upgrades", "nav-button");
			buttons.Add(new UIButton(packagesBtn, rootVisualElement.Q("upgrades-holder")));

			var socialsBtn = rootVisualElement.Q("nav-btn-socials", "nav-button");
			buttons.Add(new UIButton(socialsBtn, rootVisualElement.Q("socials-holder")));

			for (int i = 0; i < buttons.Count; i++)
            {
				int index = i;
				buttons[i].OnSelected += () => { HandleNavButtonSelection(index); };
			}

			rootVisualElement.Q<Button>(null, "pdf-btn").clicked += () =>
			{
				Process.Start(new FileInfo(AssetDatabase.GetAssetPath(manual)).FullName);
			};

			var pdfTitle = rootVisualElement.Q<TextElement>(null, "pdf-title");
			pdfTitle.text = manual.name + ".pdf";

			rootVisualElement.Q<Button>(null, "hdrp").clicked += () =>
			{
				ProjectWindowUtil.ShowCreatedAsset(hdrpUpgrade);
			};
			rootVisualElement.Q<Button>(null, "urp").clicked += () =>
			{
				ProjectWindowUtil.ShowCreatedAsset(urpUpgrade);
			};
			var toggle = rootVisualElement.Q<Toggle>("", "win-state-toggle");

			toggle.RegisterValueChangedCallback(evt => 
			{
				EditorPrefs.SetBool("WFDM_DoNotShowAtStartWelcomeWindow", evt.newValue);
			});

			var state = EditorPrefs.GetBool("WFDM_DoNotShowAtStartWelcomeWindow");
			toggle.SetValueWithoutNotify(state);
		}

		IEnumerator LoadResources()
        {
			var res = rootVisualElement.Query(null, "res").Build().ToList();

            for (int i = 0; i < res.Count; i++)
            {
				var r = (Image)res[i];

				if (!resources.ContainsKey(r.name))
				{ continue; }

				var url = resources[r.name];
				yield return EditorCoroutineRunner.StartCoroutine(util.Request(url, new DownloadHandlerTexture(), (DownloadHandler handler) =>
                {
					var tex = ((DownloadHandlerTexture)handler).texture;
					if (r.ClassListContains("invert"))
                    {
						tex = util.InvertTexture(tex);
					}
                    r.image = tex;
                }));
            }
		}

		void HandleNavButtonSelection(int index)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
				if (i == index) { continue; }

				buttons[i].Deselect();
			}
        }		

		private void OnGUI()
		{
			if (noInternet)
            {
				GUIStyle gUIStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { fontSize = 14 };
				GUI.Label(new Rect(0, 0, position.width, position.height), "No internet connection", gUIStyle);
				return;
            }
			var ve = rootVisualElement.Q<VisualElement>("", "content-container");
			if (ve != null)
			{
				ve.style.height = new StyleLength(position.height);
			}
		}
	}
}