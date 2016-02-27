﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace More {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ExceptionMessage {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ExceptionMessage() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("More.ExceptionMessage", typeof(ExceptionMessage).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value must be greater than or equal to one..
        /// </summary>
        internal static string ArgumentLessThanOne {
            get {
                return ResourceManager.GetString("ArgumentLessThanOne", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value must be greater than or equal to zero..
        /// </summary>
        internal static string ArgumentLessThanZero {
            get {
                return ResourceManager.GetString("ArgumentLessThanZero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Year, Month, and Day parameters describe an un-representable DateTime..
        /// </summary>
        internal static string ArgumentOutOfRangeBadYearMonthDay {
            get {
                return ResourceManager.GetString("ArgumentOutOfRangeBadYearMonthDay", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Era value was not valid..
        /// </summary>
        internal static string ArgumentOutOfRangeInvalidEraValue {
            get {
                return ResourceManager.GetString("ArgumentOutOfRangeInvalidEraValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Month must be between one and twelve..
        /// </summary>
        internal static string ArgumentOutOfRangeMonth {
            get {
                return ResourceManager.GetString("ArgumentOutOfRangeMonth", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Non-negative number required..
        /// </summary>
        internal static string ArgumentOutOfRangeNeedNonNegNum {
            get {
                return ResourceManager.GetString("ArgumentOutOfRangeNeedNonNegNum", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Valid values are between {0} and {1}, inclusive..
        /// </summary>
        internal static string ArgumentOutOfRangeRange {
            get {
                return ResourceManager.GetString("ArgumentOutOfRangeRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The result is out of the supported range for this calendar. The result should be between {0} (Gregorian date) and {1} (Gregorian date), inclusive..
        /// </summary>
        internal static string ArgumentResultCalendarRange {
            get {
                return ResourceManager.GetString("ArgumentResultCalendarRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified array is not of the correct type..
        /// </summary>
        internal static string ArrayMismatch {
            get {
                return ResourceManager.GetString("ArrayMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified array is too small to copy the number of elements to..
        /// </summary>
        internal static string ArrayTooSmall {
            get {
                return ResourceManager.GetString("ArrayTooSmall", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to IComparer (or the IComparable methods it relies upon) did not return zero when Array.Sort called x. CompareTo(x). x: &apos;{0}&apos;  x&apos;s type: &apos;{1}&apos; The IComparer: &apos;{2}&apos;..
        /// </summary>
        internal static string BogusIComparer {
            get {
                return ResourceManager.GetString("BogusIComparer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The object cannot end initialization before it has started..
        /// </summary>
        internal static string CannotEndInitBeforeBeginInit {
            get {
                return ResourceManager.GetString("CannotEndInitBeforeBeginInit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The object cannot be initialized more than once..
        /// </summary>
        internal static string CannotInitializeMoreThanOnce {
            get {
                return ResourceManager.GetString("CannotInitializeMoreThanOnce", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An existing item cannot be registered as a new item..
        /// </summary>
        internal static string CannotRegisterExistingItemAsNew {
            get {
                return ResourceManager.GetString("CannotRegisterExistingItemAsNew", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A removed item cannot be registered as a new item..
        /// </summary>
        internal static string CannotRegisterRemovedItemAsNew {
            get {
                return ResourceManager.GetString("CannotRegisterRemovedItemAsNew", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The current position of the enumerator is after the last element..
        /// </summary>
        internal static string CollectionAfterLast {
            get {
                return ResourceManager.GetString("CollectionAfterLast", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The current position of the enumerator is before the first element..
        /// </summary>
        internal static string CollectionBeforeFirst {
            get {
                return ResourceManager.GetString("CollectionBeforeFirst", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enumeration has either not started or has already finished..
        /// </summary>
        internal static string CollectionCannotEnumerate {
            get {
                return ResourceManager.GetString("CollectionCannotEnumerate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The collection was modified after the enumerator was created..
        /// </summary>
        internal static string CollectionModified {
            get {
                return ResourceManager.GetString("CollectionModified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The collection is read-only. Items cannot be added, removed, or substituted..
        /// </summary>
        internal static string CollectionReadOnly {
            get {
                return ResourceManager.GetString("CollectionReadOnly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The SyncRoot property may not be used for the synchronization of concurrent collections..
        /// </summary>
        internal static string ConcurrentCollection_SyncRoot_NotSupported {
            get {
                return ResourceManager.GetString("ConcurrentCollection_SyncRoot_NotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The array is multidimensional, or the type parameter for the set cannot be cast automatically to the type of the destination array..
        /// </summary>
        internal static string ConcurrentDictionary_ArrayIncorrectType {
            get {
                return ResourceManager.GetString("ConcurrentDictionary_ArrayIncorrectType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The index is equal to or greater than the length of the array, or the number of elements in the dictionary is greater than the available space from index to the end of the destination array..
        /// </summary>
        internal static string ConcurrentDictionary_ArrayNotLargeEnough {
            get {
                return ResourceManager.GetString("ConcurrentDictionary_ArrayNotLargeEnough", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The capacity argument must be greater than or equal to zero..
        /// </summary>
        internal static string ConcurrentDictionary_CapacityMustNotBeNegative {
            get {
                return ResourceManager.GetString("ConcurrentDictionary_CapacityMustNotBeNegative", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The concurrencyLevel argument must be positive..
        /// </summary>
        internal static string ConcurrentDictionary_ConcurrencyLevelMustBePositive {
            get {
                return ResourceManager.GetString("ConcurrentDictionary_ConcurrencyLevelMustBePositive", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The index argument is less than zero..
        /// </summary>
        internal static string ConcurrentDictionary_IndexIsNegative {
            get {
                return ResourceManager.GetString("ConcurrentDictionary_IndexIsNegative", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to TKey is a reference type and item.Key is null..
        /// </summary>
        internal static string ConcurrentDictionary_ItemKeyIsNull {
            get {
                return ResourceManager.GetString("ConcurrentDictionary_ItemKeyIsNull", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The key already existed in the dictionary..
        /// </summary>
        internal static string ConcurrentDictionary_KeyAlreadyExisted {
            get {
                return ResourceManager.GetString("ConcurrentDictionary_KeyAlreadyExisted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The source argument contains duplicate keys..
        /// </summary>
        internal static string ConcurrentDictionary_SourceContainsDuplicateKeys {
            get {
                return ResourceManager.GetString("ConcurrentDictionary_SourceContainsDuplicateKeys", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The key was of an incorrect type for this dictionary..
        /// </summary>
        internal static string ConcurrentDictionary_TypeOfKeyIncorrect {
            get {
                return ResourceManager.GetString("ConcurrentDictionary_TypeOfKeyIncorrect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value was of an incorrect type for this dictionary..
        /// </summary>
        internal static string ConcurrentDictionary_TypeOfValueIncorrect {
            get {
                return ResourceManager.GetString("ConcurrentDictionary_TypeOfValueIncorrect", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An unspecified error occurred..
        /// </summary>
        internal static string ConfigurationErrorsException {
            get {
                return ResourceManager.GetString("ConfigurationErrorsException", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The count must be less than or equal to the total number of elements in the sequence..
        /// </summary>
        internal static string CountTooLarge {
            get {
                return ResourceManager.GetString("CountTooLarge", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The dictionary is read-only. Items cannot be added, removed, or substituted..
        /// </summary>
        internal static string DictionaryReadOnly {
            get {
                return ResourceManager.GetString("DictionaryReadOnly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A transaction has not been started..
        /// </summary>
        internal static string EditTransactionNotStarted {
            get {
                return ResourceManager.GetString("EditTransactionNotStarted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to EndInvoke can only be called once for each asynchronous operation..
        /// </summary>
        internal static string EndInvokeCalledTwice {
            get {
                return ResourceManager.GetString("EndInvokeCalledTwice", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to EndInvoke has not been called for the current asynchronous operation..
        /// </summary>
        internal static string EndInvokeNotCalled {
            get {
                return ResourceManager.GetString("EndInvokeNotCalled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The expression &apos;{0}&apos; must refer to a property declared by type {1}..
        /// </summary>
        internal static string ExpressionMustReferToPropertyOfDeclaredType {
            get {
                return ResourceManager.GetString("ExpressionMustReferToPropertyOfDeclaredType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The expression &apos;{0}&apos; must refer to a public, instance property..
        /// </summary>
        internal static string ExpresssionMustReferToInstanceProperty {
            get {
                return ResourceManager.GetString("ExpresssionMustReferToInstanceProperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The expression &apos;{0}&apos; must refer to a property..
        /// </summary>
        internal static string ExpresssionMustReferToProperty {
            get {
                return ResourceManager.GetString("ExpresssionMustReferToProperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HashSet capacity is too big..
        /// </summary>
        internal static string HashSetCapacityOverflow {
            get {
                return ResourceManager.GetString("HashSetCapacityOverflow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to compare two elements in the array..
        /// </summary>
        internal static string IComparerFailed {
            get {
                return ResourceManager.GetString("IComparerFailed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified index is out of bounds..
        /// </summary>
        internal static string IndexOutOfRange {
            get {
                return ResourceManager.GetString("IndexOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified activity identifier is invalid. The value must be a globally unique identifier (GUID)..
        /// </summary>
        internal static string InvalidActivityId {
            get {
                return ResourceManager.GetString("InvalidActivityId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot convert type &apos;{0}&apos; to type &apos;{1}&apos;..
        /// </summary>
        internal static string InvalidCast {
            get {
                return ResourceManager.GetString("InvalidCast", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; is not a delegate type..
        /// </summary>
        internal static string InvalidDelegateType {
            get {
                return ResourceManager.GetString("InvalidDelegateType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Input string was not in a correct format..
        /// </summary>
        internal static string InvalidFormatString {
            get {
                return ResourceManager.GetString("InvalidFormatString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A required service of type &apos;{0}&apos; could not be found..
        /// </summary>
        internal static string MissingRequiredService {
            get {
                return ResourceManager.GetString("MissingRequiredService", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sequence contains more than one element..
        /// </summary>
        internal static string MoreThanOneElement {
            get {
                return ResourceManager.GetString("MoreThanOneElement", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sequence contains more than one matching element..
        /// </summary>
        internal static string MoreThanOneMatch {
            get {
                return ResourceManager.GetString("MoreThanOneMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Multiple factories have been registered for a unit of work of type &apos;{0}&apos;..
        /// </summary>
        internal static string MultipleUnitOfWorkFactory {
            get {
                return ResourceManager.GetString("MultipleUnitOfWorkFactory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The day of week must be the same as epoch ({0:G})..
        /// </summary>
        internal static string MustMatchEpochStartDay {
            get {
                return ResourceManager.GetString("MustMatchEpochStartDay", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A transaction has already been started.  Nested transactions are not supported..
        /// </summary>
        internal static string NestedEditTransactionDetected {
            get {
                return ResourceManager.GetString("NestedEditTransactionDetected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Object initialization has already started.  Nested initialization scopes are not supported..
        /// </summary>
        internal static string NestedInitializationDetected {
            get {
                return ResourceManager.GetString("NestedInitializationDetected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sequence contains no elements..
        /// </summary>
        internal static string NoElements {
            get {
                return ResourceManager.GetString("NoElements", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sequence contains no matching element..
        /// </summary>
        internal static string NoMatch {
            get {
                return ResourceManager.GetString("NoMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to When paging a sequence, the pageNumber must be at least 0..
        /// </summary>
        internal static string PageNumberMustBeGreaterThanZero {
            get {
                return ResourceManager.GetString("PageNumberMustBeGreaterThanZero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to When paging a sequence, the pageSize must be at least 1..
        /// </summary>
        internal static string PageSizeMustBeGreaterThanZero {
            get {
                return ResourceManager.GetString("PageSizeMustBeGreaterThanZero", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;{0}&apos; property is read-only..
        /// </summary>
        internal static string PropertyIsReadOnly {
            get {
                return ResourceManager.GetString("PropertyIsReadOnly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Queues do not support removing arbitrary items.  Use the Dequeue method instead..
        /// </summary>
        internal static string QueueDoesNotSupportRemove {
            get {
                return ResourceManager.GetString("QueueDoesNotSupportRemove", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The queue is empty..
        /// </summary>
        internal static string QueueIsEmpty {
            get {
                return ResourceManager.GetString("QueueIsEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Stacks do not support removing arbitrary items.  Use the Pop method instead..
        /// </summary>
        internal static string StackDoesNotSupportRemove {
            get {
                return ResourceManager.GetString("StackDoesNotSupportRemove", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The stack is empty..
        /// </summary>
        internal static string StackIsEmpty {
            get {
                return ResourceManager.GetString("StackIsEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No factory has been registered for a unit of work of type &apos;{0}&apos;..
        /// </summary>
        internal static string UnmappedUnitOfWorkFactory {
            get {
                return ResourceManager.GetString("UnmappedUnitOfWorkFactory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to IAsyncResult object did not come from the corresponding async method on this type..
        /// </summary>
        internal static string WrongIAsyncResult {
            get {
                return ResourceManager.GetString("WrongIAsyncResult", resourceCulture);
            }
        }
    }
}
