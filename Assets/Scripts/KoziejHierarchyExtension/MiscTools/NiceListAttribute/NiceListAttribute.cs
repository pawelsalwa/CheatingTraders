using System;

namespace AWI {
	 [AttributeUsage(AttributeTargets.Field)]
	 public class NiceListAttribute : Attribute {
		  public NiceListMode mode {
				get;
				private set;
		  }
		  public bool forceExpand {
				get;
				private set;
		  }
		  public bool compact {
				get;
				private set;
		  }
		  public NiceListAttribute(NiceListMode mode, bool usePreviewToEdit = false, bool forceExpand = false) {
				this.mode = mode;
				this.forceExpand = forceExpand;
				this.compact = usePreviewToEdit;
		  }
		  public NiceListAttribute() : this(NiceListMode.Flexible, false, false) {
		  }
		  public static NiceListAttribute defaultAttribute {
				get {
					 if(m_DefaultAttribute == null) {
						  m_DefaultAttribute = new NiceListAttribute(NiceListMode.Flexible, false);
					 }
					 return m_DefaultAttribute;
				}
		  }
		  private static NiceListAttribute m_DefaultAttribute = null;
	 }
	 
}