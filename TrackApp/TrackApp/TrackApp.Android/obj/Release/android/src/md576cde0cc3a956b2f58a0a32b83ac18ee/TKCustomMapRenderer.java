package md576cde0cc3a956b2f58a0a32b83ac18ee;


public class TKCustomMapRenderer
	extends md51558244f76c53b6aeda52c8a337f2c37.ViewRenderer_2
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.maps.GoogleMap.SnapshotReadyCallback,
		com.google.android.gms.maps.GoogleMap.OnCameraIdleListener,
		com.google.android.gms.maps.OnMapReadyCallback
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onLayout:(ZIIII)V:GetOnLayout_ZIIIIHandler\n" +
			"n_onSnapshotReady:(Landroid/graphics/Bitmap;)V:GetOnSnapshotReady_Landroid_graphics_Bitmap_Handler:Android.Gms.Maps.GoogleMap/ISnapshotReadyCallbackInvoker, Xamarin.GooglePlayServices.Maps\n" +
			"n_onCameraIdle:()V:GetOnCameraIdleHandler:Android.Gms.Maps.GoogleMap/IOnCameraIdleListenerInvoker, Xamarin.GooglePlayServices.Maps\n" +
			"n_onMapReady:(Lcom/google/android/gms/maps/GoogleMap;)V:GetOnMapReady_Lcom_google_android_gms_maps_GoogleMap_Handler:Android.Gms.Maps.IOnMapReadyCallbackInvoker, Xamarin.GooglePlayServices.Maps\n" +
			"";
		mono.android.Runtime.register ("TK.CustomMap.Droid.TKCustomMapRenderer, TK.CustomMap.Android", TKCustomMapRenderer.class, __md_methods);
	}


	public TKCustomMapRenderer (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == TKCustomMapRenderer.class)
			mono.android.TypeManager.Activate ("TK.CustomMap.Droid.TKCustomMapRenderer, TK.CustomMap.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public TKCustomMapRenderer (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == TKCustomMapRenderer.class)
			mono.android.TypeManager.Activate ("TK.CustomMap.Droid.TKCustomMapRenderer, TK.CustomMap.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public TKCustomMapRenderer (android.content.Context p0)
	{
		super (p0);
		if (getClass () == TKCustomMapRenderer.class)
			mono.android.TypeManager.Activate ("TK.CustomMap.Droid.TKCustomMapRenderer, TK.CustomMap.Android", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public void onLayout (boolean p0, int p1, int p2, int p3, int p4)
	{
		n_onLayout (p0, p1, p2, p3, p4);
	}

	private native void n_onLayout (boolean p0, int p1, int p2, int p3, int p4);


	public void onSnapshotReady (android.graphics.Bitmap p0)
	{
		n_onSnapshotReady (p0);
	}

	private native void n_onSnapshotReady (android.graphics.Bitmap p0);


	public void onCameraIdle ()
	{
		n_onCameraIdle ();
	}

	private native void n_onCameraIdle ();


	public void onMapReady (com.google.android.gms.maps.GoogleMap p0)
	{
		n_onMapReady (p0);
	}

	private native void n_onMapReady (com.google.android.gms.maps.GoogleMap p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
