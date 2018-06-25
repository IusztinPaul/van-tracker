package md5c64149f41d11c1c3c15290e55f06a859;


public class LocationSBinder
	extends android.os.Binder
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("TrackApp.Droid.LocationSBinder, TrackApp.Android", LocationSBinder.class, __md_methods);
	}


	public LocationSBinder ()
	{
		super ();
		if (getClass () == LocationSBinder.class)
			mono.android.TypeManager.Activate ("TrackApp.Droid.LocationSBinder, TrackApp.Android", "", this, new java.lang.Object[] {  });
	}

	public LocationSBinder (com.etheral.LocationService p0)
	{
		super ();
		if (getClass () == LocationSBinder.class)
			mono.android.TypeManager.Activate ("TrackApp.Droid.LocationSBinder, TrackApp.Android", "TrackApp.Droid.LocationService, TrackApp.Android", this, new java.lang.Object[] { p0 });
	}

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
