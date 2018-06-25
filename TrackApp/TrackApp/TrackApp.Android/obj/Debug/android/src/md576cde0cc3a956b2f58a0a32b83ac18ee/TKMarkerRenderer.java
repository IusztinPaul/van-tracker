package md576cde0cc3a956b2f58a0a32b83ac18ee;


public class TKMarkerRenderer
	extends com.google.maps.android.clustering.view.DefaultClusterRenderer
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onBeforeClusterItemRendered:(Lcom/google/maps/android/clustering/ClusterItem;Lcom/google/android/gms/maps/model/MarkerOptions;)V:GetOnBeforeClusterItemRendered_Lcom_google_maps_android_clustering_ClusterItem_Lcom_google_android_gms_maps_model_MarkerOptions_Handler\n" +
			"n_onClusterItemRendered:(Lcom/google/maps/android/clustering/ClusterItem;Lcom/google/android/gms/maps/model/Marker;)V:GetOnClusterItemRendered_Lcom_google_maps_android_clustering_ClusterItem_Lcom_google_android_gms_maps_model_Marker_Handler\n" +
			"n_onBeforeClusterRendered:(Lcom/google/maps/android/clustering/Cluster;Lcom/google/android/gms/maps/model/MarkerOptions;)V:GetOnBeforeClusterRendered_Lcom_google_maps_android_clustering_Cluster_Lcom_google_android_gms_maps_model_MarkerOptions_Handler\n" +
			"n_onClusterRendered:(Lcom/google/maps/android/clustering/Cluster;Lcom/google/android/gms/maps/model/Marker;)V:GetOnClusterRendered_Lcom_google_maps_android_clustering_Cluster_Lcom_google_android_gms_maps_model_Marker_Handler\n" +
			"";
		mono.android.Runtime.register ("TK.CustomMap.Droid.TKMarkerRenderer, TK.CustomMap.Android", TKMarkerRenderer.class, __md_methods);
	}


	public TKMarkerRenderer (android.content.Context p0, com.google.android.gms.maps.GoogleMap p1, com.google.maps.android.clustering.ClusterManager p2)
	{
		super (p0, p1, p2);
		if (getClass () == TKMarkerRenderer.class)
			mono.android.TypeManager.Activate ("TK.CustomMap.Droid.TKMarkerRenderer, TK.CustomMap.Android", "Android.Content.Context, Mono.Android:Android.Gms.Maps.GoogleMap, Xamarin.GooglePlayServices.Maps:Com.Google.Maps.Android.Clustering.ClusterManager, GoogleMapsUtilityBinding", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public void onBeforeClusterItemRendered (com.google.maps.android.clustering.ClusterItem p0, com.google.android.gms.maps.model.MarkerOptions p1)
	{
		n_onBeforeClusterItemRendered (p0, p1);
	}

	private native void n_onBeforeClusterItemRendered (com.google.maps.android.clustering.ClusterItem p0, com.google.android.gms.maps.model.MarkerOptions p1);


	public void onClusterItemRendered (com.google.maps.android.clustering.ClusterItem p0, com.google.android.gms.maps.model.Marker p1)
	{
		n_onClusterItemRendered (p0, p1);
	}

	private native void n_onClusterItemRendered (com.google.maps.android.clustering.ClusterItem p0, com.google.android.gms.maps.model.Marker p1);


	public void onBeforeClusterRendered (com.google.maps.android.clustering.Cluster p0, com.google.android.gms.maps.model.MarkerOptions p1)
	{
		n_onBeforeClusterRendered (p0, p1);
	}

	private native void n_onBeforeClusterRendered (com.google.maps.android.clustering.Cluster p0, com.google.android.gms.maps.model.MarkerOptions p1);


	public void onClusterRendered (com.google.maps.android.clustering.Cluster p0, com.google.android.gms.maps.model.Marker p1)
	{
		n_onClusterRendered (p0, p1);
	}

	private native void n_onClusterRendered (com.google.maps.android.clustering.Cluster p0, com.google.android.gms.maps.model.Marker p1);

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
