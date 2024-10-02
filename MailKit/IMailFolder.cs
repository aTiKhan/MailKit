﻿//
// IMailFolder.cs
//
// Author: Jeffrey Stedfast <jestedfa@microsoft.com>
//
// Copyright (c) 2013-2024 .NET Foundation and Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using MimeKit;
using MailKit.Search;

#if NET5_0_OR_GREATER
using IReadOnlySetOfStrings = System.Collections.Generic.IReadOnlySet<string>;
#else
using IReadOnlySetOfStrings = System.Collections.Generic.ISet<string>;
#endif

namespace MailKit {
	/// <summary>
	/// An interface for a mailbox folder as used by <see cref="IMailStore"/>.
	/// </summary>
	/// <remarks>
	/// Implemented by message stores such as <see cref="MailKit.Net.Imap.ImapClient"/>
	/// </remarks>
	public interface IMailFolder : IEnumerable<MimeMessage>
	{
		/// <summary>
		/// Gets an object that can be used to synchronize access to the folder.
		/// </summary>
		/// <remarks>
		/// Gets an object that can be used to synchronize access to the folder.
		/// </remarks>
		/// <value>The sync root.</value>
		object SyncRoot { get; }

		/// <summary>
		/// Get the parent folder.
		/// </summary>
		/// <remarks>
		/// Root-level folders do not have a parent folder.
		/// </remarks>
		/// <value>The parent folder.</value>
		IMailFolder ParentFolder { get; }

		/// <summary>
		/// Get the folder attributes.
		/// </summary>
		/// <remarks>
		/// Gets the folder attributes.
		/// </remarks>
		/// <value>The folder attributes.</value>
		FolderAttributes Attributes { get; }

		/// <summary>
		/// Get the annotation access level.
		/// </summary>
		/// <remarks>
		/// If annotations are supported, this property can be used to determine whether or not
		/// the <see cref="IMailFolder"/> supports reading and writing annotations.
		/// </remarks>
		/// <value>The annotation access level.</value>
		AnnotationAccess AnnotationAccess { get; }

		/// <summary>
		/// Get the supported annotation scopes.
		/// </summary>
		/// <remarks>
		/// If annotations are supported, this property can be used to determine which
		/// annotation scopes are supported by the <see cref="IMailFolder"/>.
		/// </remarks>
		/// <value>The supported annotation scopes.</value>
		AnnotationScope AnnotationScopes { get; }

		/// <summary>
		/// Get the maximum size of annotation values supported by the folder.
		/// </summary>
		/// <remarks>
		/// If annotations are supported, this property can be used to determine the
		/// maximum size of annotation values supported by the <see cref="IMailFolder"/>.
		/// </remarks>
		/// <value>The maximum size of annotation values supported by the folder.</value>
		uint MaxAnnotationSize { get; }

		/// <summary>
		/// Get the permanent flags.
		/// </summary>
		/// <remarks>
		/// <para>The permanent flags are the message flags that will persist between sessions.</para>
		/// <para>If the <see cref="MessageFlags.UserDefined"/> flag is set, then the folder allows
		/// storing of user-defined (custom) message flags.</para>
		/// </remarks>
		/// <value>The permanent flags.</value>
		MessageFlags PermanentFlags { get; }

		/// <summary>
		/// Get the permanent keywords.
		/// </summary>
		/// <remarks>
		/// <para>The permanent keywords are the keywords that will persist between sessions.</para>
		/// <para>If the <see cref="MessageFlags.UserDefined"/> flag is set in <see cref="PermanentFlags"/>,
		/// then the folder allows storing of user-defined keywords as well.</para>
		/// </remarks>
		/// <value>The permanent keywords.</value>
		IReadOnlySetOfStrings PermanentKeywords { get; }

		/// <summary>
		/// Get the accepted flags.
		/// </summary>
		/// <remarks>
		/// The accepted flags are the message flags that will be accepted and persist
		/// for the current session. For the set of flags that will persist between
		/// sessions, see the <see cref="PermanentFlags"/> property.
		/// </remarks>
		/// <value>The accepted flags.</value>
		MessageFlags AcceptedFlags { get; }

		/// <summary>
		/// Get the accepted keywords.
		/// </summary>
		/// <remarks>
		/// The accepted keywords are the keywords that will be accepted and persist
		/// for the current session. For the set of keywords that will persist between
		/// sessions, see the <see cref="PermanentKeywords"/> property.
		/// </remarks>
		/// <value>The accepted keywords.</value>
		IReadOnlySetOfStrings AcceptedKeywords { get; }

		/// <summary>
		/// Get the directory separator.
		/// </summary>
		/// <remarks>
		/// Gets the directory separator.
		/// </remarks>
		/// <value>The directory separator.</value>
		char DirectorySeparator { get; }

		/// <summary>
		/// Get the read/write access of the folder.
		/// </summary>
		/// <remarks>
		/// Gets the read/write access of the folder.
		/// </remarks>
		/// <value>The read/write access.</value>
		FolderAccess Access { get; }

		/// <summary>
		/// Get whether or not the folder is a namespace folder.
		/// </summary>
		/// <remarks>
		/// Gets whether or not the folder is a namespace folder.
		/// </remarks>
		/// <value><c>true</c> if the folder is a namespace folder; otherwise, <c>false</c>.</value>
		bool IsNamespace { get; }

		/// <summary>
		/// Get the full name of the folder.
		/// </summary>
		/// <remarks>
		/// This is the equivalent of the full path of a file on a file system.
		/// </remarks>
		/// <value>The full name of the folder.</value>
		string FullName { get; }

		/// <summary>
		/// Get the name of the folder.
		/// </summary>
		/// <remarks>
		/// This is the equivalent of the file name of a file on the file system.
		/// </remarks>
		/// <value>The name of the folder.</value>
		string Name { get; }

		/// <summary>
		/// Get the unique identifier for the folder, if available.
		/// </summary>
		/// <remarks>
		/// <para>Gets a unique identifier for the folder, if available. This is useful for clients
		/// implementing a message cache that want to track the folder after it is renamed by another
		/// client.</para>
		/// <note type="note">This property will only be available if the server supports the
		/// <a href="https://tools.ietf.org/html/rfc8474">OBJECTID</a> extension.</note>
		/// </remarks>
		/// <value>The unique folder identifier.</value>
		string Id { get; }

		/// <summary>
		/// Get whether or not the folder is subscribed.
		/// </summary>
		/// <remarks>
		/// Gets whether or not the folder is subscribed.
		/// </remarks>
		/// <value><c>true</c> if the folder is subscribed; otherwise, <c>false</c>.</value>
		bool IsSubscribed { get; }

		/// <summary>
		/// Get whether or not the folder is currently open.
		/// </summary>
		/// <remarks>
		/// Gets whether or not the folder is currently open.
		/// </remarks>
		/// <value><c>true</c> if the folder is currently open; otherwise, <c>false</c>.</value>
		bool IsOpen { get; }

		/// <summary>
		/// Get whether or not the folder exists.
		/// </summary>
		/// <remarks>
		/// Gets whether or not the folder exists.
		/// </remarks>
		/// <value><c>true</c> if the folder exists; otherwise, <c>false</c>.</value>
		bool Exists { get; }

		/// <summary>
		/// Get the highest mod-sequence value of all messages in the mailbox.
		/// </summary>
		/// <remarks>
		/// Gets the highest mod-sequence value of all messages in the mailbox.
		/// </remarks>
		/// <value>The highest mod-sequence value.</value>
		ulong HighestModSeq { get; }

		/// <summary>
		/// Get the Unique ID validity.
		/// </summary>
		/// <remarks>
		/// <para>UIDs are only valid so long as the UID validity value remains unchanged. If and when
		/// the folder's <see cref="UidValidity"/> is changed, a client MUST discard its cache of UIDs
		/// along with any summary information that it may have and re-query the folder.</para>
		/// <para>The <see cref="UidValidity"/> will only be set after the folder has been opened.</para>
		/// </remarks>
		/// <value>The UID validity.</value>
		uint UidValidity { get; }

		/// <summary>
		/// Get the UID that the next message that is added to the folder will be assigned.
		/// </summary>
		/// <remarks>
		/// This value will only be set after the folder has been opened.
		/// </remarks>
		/// <value>The next UID.</value>
		UniqueId? UidNext { get; }

		/// <summary>
		/// Get the maximum size of a message that can be appended to the folder.
		/// </summary>
		/// <remarks>
		/// <para>Gets the maximum size of a message that can be appended to the folder.</para>
		/// <note type="note">If the value is not set, then the limit is unspecified.</note>
		/// </remarks>
		/// <value>The append limit.</value>
		uint? AppendLimit { get; }

		/// <summary>
		/// Get the size of the folder.
		/// </summary>
		/// <remarks>
		/// <para>Gets the size of the folder in bytes.</para>
		/// <note type="note">If the value is not set, then the size is unspecified.</note>
		/// </remarks>
		/// <value>The size.</value>
		ulong? Size { get; }

		/// <summary>
		/// Get the index of the first unread message in the folder.
		/// </summary>
		/// <remarks>
		/// This value will only be set after the folder has been opened.
		/// </remarks>
		/// <value>The index of the first unread message.</value>
		int FirstUnread { get; }

		/// <summary>
		/// Get the number of unread messages in the folder.
		/// </summary>
		/// <remarks>
		/// <para>Gets the number of unread messages in the folder.</para>
		/// <note type="note">This value will only be set after calling
		/// <see cref="Status(StatusItems, System.Threading.CancellationToken)"/>
		/// with <see cref="StatusItems.Unread"/>.</note>
		/// </remarks>
		/// <value>The number of unread messages.</value>
		int Unread { get; }

		/// <summary>
		/// Get the number of recently delivered messages in the folder.
		/// </summary>
		/// <remarks>
		/// <para>Gets the number of recently delivered messages in the folder.</para>
		/// <note type="note">
		/// This value will only be set after calling
		/// <see cref="Status(StatusItems, System.Threading.CancellationToken)"/>
		/// with <see cref="StatusItems.Recent"/>.</note>
		/// </remarks>
		/// <value>The number of recently delivered messages.</value>
		int Recent { get; }

		/// <summary>
		/// Get the total number of messages in the folder.
		/// </summary>
		/// <remarks>
		/// Gets the total number of messages in the folder.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\ImapExamples.cs" region="DownloadMessagesByIndex"/>
		/// </example>
		/// <value>The total number of messages.</value>
		int Count { get; }

		/// <summary>
		/// Get the threading algorithms supported by the folder.
		/// </summary>
		/// <remarks>
		/// Get the threading algorithms supported by the folder.
		/// </remarks>
		/// <value>The supported threading algorithms.</value>
		HashSet<ThreadingAlgorithm> ThreadingAlgorithms { get; }

		/// <summary>
		/// Determine whether or not an <see cref="IMailFolder"/> supports a feature.
		/// </summary>
		/// <remarks>
		/// Determines whether or not an <see cref="IMailFolder"/> supports a feature.
		/// </remarks>
		/// <param name="feature">The desired feature.</param>
		/// <returns><c>true</c> if the feature is supported; otherwise, <c>false</c>.</returns>
		bool Supports (FolderFeature feature);

		/// <summary>
		/// Opens the folder using the requested folder access.
		/// </summary>
		/// <remarks>
		/// <para>This variant of the <see cref="Open(FolderAccess,System.Threading.CancellationToken)"/>
		/// method is meant for quick resynchronization of the folder. Before calling this method,
		/// the <see cref="IMailStore.EnableQuickResync(CancellationToken)"/> method MUST be called.</para>
		/// <para>You should also make sure to add listeners to the <see cref="MessagesVanished"/> and
		/// <see cref="MessageFlagsChanged"/> events to get notifications of changes since
		/// the last time the folder was opened.</para>
		/// </remarks>
		/// <returns>The <see cref="FolderAccess"/> state of the folder.</returns>
		/// <param name="access">The requested folder access.</param>
		/// <param name="uidValidity">The last known <see cref="UidValidity"/> value.</param>
		/// <param name="highestModSeq">The last known <see cref="HighestModSeq"/> value.</param>
		/// <param name="uids">The last known list of unique message identifiers.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		FolderAccess Open (FolderAccess access, uint uidValidity, ulong highestModSeq, IList<UniqueId> uids, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously opens the folder using the requested folder access.
		/// </summary>
		/// <remarks>
		/// <para>This variant of the <see cref="OpenAsync(FolderAccess,System.Threading.CancellationToken)"/>
		/// method is meant for quick resynchronization of the folder. Before calling this method,
		/// the <see cref="IMailStore.EnableQuickResync(CancellationToken)"/> method MUST be called.</para>
		/// <para>You should also make sure to add listeners to the <see cref="MessagesVanished"/> and
		/// <see cref="MessageFlagsChanged"/> events to get notifications of changes since
		/// the last time the folder was opened.</para>
		/// </remarks>
		/// <returns>The <see cref="FolderAccess"/> state of the folder.</returns>
		/// <param name="access">The requested folder access.</param>
		/// <param name="uidValidity">The last known <see cref="UidValidity"/> value.</param>
		/// <param name="highestModSeq">The last known <see cref="HighestModSeq"/> value.</param>
		/// <param name="uids">The last known list of unique message identifiers.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<FolderAccess> OpenAsync (FolderAccess access, uint uidValidity, ulong highestModSeq, IList<UniqueId> uids, CancellationToken cancellationToken = default);

		/// <summary>
		/// Open the folder using the requested folder access.
		/// </summary>
		/// <remarks>
		/// Opens the folder using the requested folder access.
		/// </remarks>
		/// <returns>The <see cref="FolderAccess"/> state of the folder.</returns>
		/// <param name="access">The requested folder access.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		FolderAccess Open (FolderAccess access, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously open the folder using the requested folder access.
		/// </summary>
		/// <remarks>
		/// Asynchronously opens the folder using the requested folder access.
		/// </remarks>
		/// <returns>The <see cref="FolderAccess"/> state of the folder.</returns>
		/// <param name="access">The requested folder access.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<FolderAccess> OpenAsync (FolderAccess access, CancellationToken cancellationToken = default);

		/// <summary>
		/// Close the folder, optionally expunging the messages marked for deletion.
		/// </summary>
		/// <remarks>
		/// Closes the folder, optionally expunging the messages marked for deletion.
		/// </remarks>
		/// <param name="expunge">If set to <c>true</c>, expunge.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		void Close (bool expunge = false, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously close the folder, optionally expunging the messages marked for deletion.
		/// </summary>
		/// <remarks>
		/// Asynchronously closes the folder, optionally expunging the messages marked for deletion.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="expunge">If set to <c>true</c>, expunge.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task CloseAsync (bool expunge = false, CancellationToken cancellationToken = default);

		/// <summary>
		/// Create a new subfolder with the given name.
		/// </summary>
		/// <remarks>
		/// Creates a new subfolder with the given name.
		/// </remarks>
		/// <returns>The created folder.</returns>
		/// <param name="name">The name of the folder to create.</param>
		/// <param name="isMessageFolder"><c>true</c> if the folder will be used to contain messages; otherwise, <c>false</c>.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IMailFolder Create (string name, bool isMessageFolder, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously create a new subfolder with the given name.
		/// </summary>
		/// <remarks>
		/// Asynchronously creates a new subfolder with the given name.
		/// </remarks>
		/// <returns>The created folder.</returns>
		/// <param name="name">The name of the folder to create.</param>
		/// <param name="isMessageFolder"><c>true</c> if the folder will be used to contain messages; otherwise, <c>false</c>.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IMailFolder> CreateAsync (string name, bool isMessageFolder, CancellationToken cancellationToken = default);

		/// <summary>
		/// Create a new subfolder with the given name.
		/// </summary>
		/// <remarks>
		/// Creates a new subfolder with the given name.
		/// </remarks>
		/// <returns>The created folder.</returns>
		/// <param name="name">The name of the folder to create.</param>
		/// <param name="specialUses">A list of special uses for the folder being created.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IMailFolder Create (string name, IEnumerable<SpecialFolder> specialUses, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously create a new subfolder with the given name.
		/// </summary>
		/// <remarks>
		/// Asynchronously creates a new subfolder with the given name.
		/// </remarks>
		/// <returns>The created folder.</returns>
		/// <param name="name">The name of the folder to create.</param>
		/// <param name="specialUses">A list of special uses for the folder being created.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IMailFolder> CreateAsync (string name, IEnumerable<SpecialFolder> specialUses, CancellationToken cancellationToken = default);

		/// <summary>
		/// Create a new subfolder with the given name.
		/// </summary>
		/// <remarks>
		/// Creates a new subfolder with the given name.
		/// </remarks>
		/// <returns>The created folder.</returns>
		/// <param name="name">The name of the folder to create.</param>
		/// <param name="specialUse">The special use for the folder being created.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IMailFolder Create (string name, SpecialFolder specialUse, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously create a new subfolder with the given name.
		/// </summary>
		/// <remarks>
		/// Asynchronously creates a new subfolder with the given name.
		/// </remarks>
		/// <returns>The created folder.</returns>
		/// <param name="name">The name of the folder to create.</param>
		/// <param name="specialUse">The special use for the folder being created.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IMailFolder> CreateAsync (string name, SpecialFolder specialUse, CancellationToken cancellationToken = default);

		/// <summary>
		/// Rename the folder.
		/// </summary>
		/// <remarks>
		/// Renames the folder.
		/// </remarks>
		/// <param name="parent">The new parent folder.</param>
		/// <param name="name">The new name of the folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		void Rename (IMailFolder parent, string name, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously rename the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously renames the folder.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="parent">The new parent folder.</param>
		/// <param name="name">The new name of the folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task RenameAsync (IMailFolder parent, string name, CancellationToken cancellationToken = default);

		/// <summary>
		/// Delete the folder.
		/// </summary>
		/// <remarks>
		/// Deletes the folder.
		/// </remarks>
		/// <param name="cancellationToken">The cancellation token.</param>
		void Delete (CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously delete the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously deletes the folder.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task DeleteAsync (CancellationToken cancellationToken = default);

		/// <summary>
		/// Subscribe to the folder.
		/// </summary>
		/// <remarks>
		/// Subscribes to the folder.
		/// </remarks>
		/// <param name="cancellationToken">The cancellation token.</param>
		void Subscribe (CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously subscribe to the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously subscribes to the folder.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task SubscribeAsync (CancellationToken cancellationToken = default);

		/// <summary>
		/// Unsubscribe from the folder.
		/// </summary>
		/// <remarks>
		/// Unsubscribes from the folder.
		/// </remarks>
		/// <param name="cancellationToken">The cancellation token.</param>
		void Unsubscribe (CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously unsubscribe from the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously unsubscribes from the folder.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task UnsubscribeAsync (CancellationToken cancellationToken = default);

		/// <summary>
		/// Get the subfolders.
		/// </summary>
		/// <remarks>
		/// <para>Gets the subfolders as well as queries the server for the status of the requested items.</para>
		/// <para>When the <paramref name="items"/> argument is non-empty, this has the equivalent functionality
		/// of calling <see cref="GetSubfolders(bool,System.Threading.CancellationToken)"/> and then calling
		/// <see cref="Status(StatusItems,System.Threading.CancellationToken)"/> on each of the returned folders.</para>
		/// <note type="tip">Using this method is potentially more efficient than querying the status of each returned folder.</note>
		/// </remarks>
		/// <returns>The subfolders.</returns>
		/// <param name="items">The status items to pre-populate.</param>
		/// <param name="subscribedOnly">If set to <c>true</c>, only subscribed folders will be listed.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IList<IMailFolder> GetSubfolders (StatusItems items, bool subscribedOnly = false, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously get the subfolders.
		/// </summary>
		/// <remarks>
		/// <para>Asynchronously gets the subfolders as well as queries the server for the status of the requested items.</para>
		/// <para>When the <paramref name="items"/> argument is non-empty, this has the equivalent functionality
		/// of calling <see cref="GetSubfoldersAsync(bool,System.Threading.CancellationToken)"/> and then calling
		/// <see cref="StatusAsync(StatusItems,System.Threading.CancellationToken)"/> on each of the returned folders.</para>
		/// </remarks>
		/// <returns>The subfolders.</returns>
		/// <param name="items">The status items to pre-populate.</param>
		/// <param name="subscribedOnly">If set to <c>true</c>, only subscribed folders will be listed.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IList<IMailFolder>> GetSubfoldersAsync (StatusItems items, bool subscribedOnly = false, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get the subfolders.
		/// </summary>
		/// <remarks>
		/// Gets the subfolders.
		/// </remarks>
		/// <returns>The subfolders.</returns>
		/// <param name="subscribedOnly">If set to <c>true</c>, only subscribed folders will be listed.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IList<IMailFolder> GetSubfolders (bool subscribedOnly = false, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously get the subfolders.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the subfolders.
		/// </remarks>
		/// <returns>The subfolders.</returns>
		/// <param name="subscribedOnly">If set to <c>true</c>, only subscribed folders will be listed.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IList<IMailFolder>> GetSubfoldersAsync (bool subscribedOnly = false, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get the specified subfolder.
		/// </summary>
		/// <remarks>
		/// Gets the specified subfolder.
		/// </remarks>
		/// <returns>The subfolder.</returns>
		/// <param name="name">The name of the subfolder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IMailFolder GetSubfolder (string name, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously get the specified subfolder.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the specified subfolder.
		/// </remarks>
		/// <returns>The subfolder.</returns>
		/// <param name="name">The name of the subfolder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IMailFolder> GetSubfolderAsync (string name, CancellationToken cancellationToken = default);

		/// <summary>
		/// Force the server to flush its state for the folder.
		/// </summary>
		/// <remarks>
		/// Forces the server to flush its state for the folder.
		/// </remarks>
		/// <param name="cancellationToken">The cancellation token.</param>
		void Check (CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously force the server to flush its state for the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously forces the server to flush its state for the folder.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task CheckAsync (CancellationToken cancellationToken = default);

		/// <summary>
		/// Update the values of the specified items.
		/// </summary>
		/// <remarks>
		/// <para>Updates the values of the specified items.</para>
		/// <para>The <see cref="Status(StatusItems, System.Threading.CancellationToken)"/> method
		/// MUST NOT be used on a folder that is already in the opened state. Instead, other ways
		/// of getting the desired information should be used.</para>
		/// <para>For example, a common use for the <see cref="Status(StatusItems,System.Threading.CancellationToken)"/>
		/// method is to get the number of unread messages in the folder. When the folder is open, however, it is
		/// possible to use the <see cref="IMailFolder.Search(MailKit.Search.SearchQuery, System.Threading.CancellationToken)"/>
		/// method to query for the list of unread messages.</para>
		/// </remarks>
		/// <param name="items">The items to update.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		void Status (StatusItems items, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously update the values of the specified items.
		/// </summary>
		/// <remarks>
		/// <para>Updates the values of the specified items.</para>
		/// <para>The <see cref="Status(StatusItems, System.Threading.CancellationToken)"/> method
		/// MUST NOT be used on a folder that is already in the opened state. Instead, other ways
		/// of getting the desired information should be used.</para>
		/// <para>For example, a common use for the <see cref="Status(StatusItems,System.Threading.CancellationToken)"/>
		/// method is to get the number of unread messages in the folder. When the folder is open, however, it is
		/// possible to use the <see cref="IMailFolder.Search(MailKit.Search.SearchQuery, System.Threading.CancellationToken)"/>
		/// method to query for the list of unread messages.</para>
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="items">The items to update.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task StatusAsync (StatusItems items, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get the complete access control list for the folder.
		/// </summary>
		/// <remarks>
		/// Gets the complete access control list for the folder.
		/// </remarks>
		/// <returns>The access control list.</returns>
		/// <param name="cancellationToken">The cancellation token.</param>
		AccessControlList GetAccessControlList (CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously get the complete access control list for the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the complete access control list for the folder.
		/// </remarks>
		/// <returns>The access control list.</returns>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<AccessControlList> GetAccessControlListAsync (CancellationToken cancellationToken = default);

		/// <summary>
		/// Get the access rights for a particular identifier.
		/// </summary>
		/// <remarks>
		/// Gets the access rights for a particular identifier.
		/// </remarks>
		/// <returns>The access rights.</returns>
		/// <param name="name">The identifier name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		AccessRights GetAccessRights (string name, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously get the access rights for a particular identifier.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the access rights for a particular identifier.
		/// </remarks>
		/// <returns>The access rights.</returns>
		/// <param name="name">The identifier name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<AccessRights> GetAccessRightsAsync (string name, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get the access rights for the current authenticated user.
		/// </summary>
		/// <remarks>
		/// Gets the access rights for the current authenticated user.
		/// </remarks>
		/// <returns>The access rights.</returns>
		/// <param name="cancellationToken">The cancellation token.</param>
		AccessRights GetMyAccessRights (CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously get the access rights for the current authenticated user.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the access rights for the current authenticated user.
		/// </remarks>
		/// <returns>The access rights.</returns>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<AccessRights> GetMyAccessRightsAsync (CancellationToken cancellationToken = default);

		/// <summary>
		/// Add access rights for the specified identity.
		/// </summary>
		/// <remarks>
		/// Adds the given access rights for the specified identity.
		/// </remarks>
		/// <param name="name">The identity name.</param>
		/// <param name="rights">The access rights.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		void AddAccessRights (string name, AccessRights rights, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously add access rights for the specified identity.
		/// </summary>
		/// <remarks>
		/// Asynchronously adds the given access rights for the specified identity.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="name">The identity name.</param>
		/// <param name="rights">The access rights.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task AddAccessRightsAsync (string name, AccessRights rights, CancellationToken cancellationToken = default);

		/// <summary>
		/// Remove access rights for the specified identity.
		/// </summary>
		/// <remarks>
		/// Removes the given access rights for the specified identity.
		/// </remarks>
		/// <param name="name">The identity name.</param>
		/// <param name="rights">The access rights.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		void RemoveAccessRights (string name, AccessRights rights, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously remove access rights for the specified identity.
		/// </summary>
		/// <remarks>
		/// Asynchronously removes the given access rights for the specified identity.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="name">The identity name.</param>
		/// <param name="rights">The access rights.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task RemoveAccessRightsAsync (string name, AccessRights rights, CancellationToken cancellationToken = default);

		/// <summary>
		/// Set the access rights for the specified identity.
		/// </summary>
		/// <remarks>
		/// Sets the access rights for the specified identity.
		/// </remarks>
		/// <param name="name">The identity name.</param>
		/// <param name="rights">The access rights.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		void SetAccessRights (string name, AccessRights rights, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously set the access rights for the sepcified identity.
		/// </summary>
		/// <remarks>
		/// Asynchronously sets the access rights for the specified identity.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="name">The identity name.</param>
		/// <param name="rights">The access rights.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task SetAccessRightsAsync (string name, AccessRights rights, CancellationToken cancellationToken = default);

		/// <summary>
		/// Remove all access rights for the given identity.
		/// </summary>
		/// <remarks>
		/// Removes all access rights for the given identity.
		/// </remarks>
		/// <param name="name">The identity name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		void RemoveAccess (string name, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously remove all access rights for the given identity.
		/// </summary>
		/// <remarks>
		/// Asynchronously removes all access rights for the given identity.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="name">The identity name.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task RemoveAccessAsync (string name, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get the quota information for the folder.
		/// </summary>
		/// <remarks>
		/// <para>Gets the quota information for the folder.</para>
		/// <para>To determine if a quotas are supported, check the 
		/// <see cref="IMailStore.SupportsQuotas"/> property.</para>
		/// </remarks>
		/// <returns>The folder quota.</returns>
		/// <param name="cancellationToken">The cancellation token.</param>
		FolderQuota GetQuota (CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously get the quota information for the folder.
		/// </summary>
		/// <remarks>
		/// <para>Asynchronously gets the quota information for the folder.</para>
		/// <para>To determine if a quotas are supported, check the 
		/// <see cref="IMailStore.SupportsQuotas"/> property.</para>
		/// </remarks>
		/// <returns>The folder quota.</returns>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<FolderQuota> GetQuotaAsync (CancellationToken cancellationToken = default);

		/// <summary>
		/// Set the quota limits for the folder.
		/// </summary>
		/// <remarks>
		/// <para>Sets the quota limits for the folder.</para>
		/// <para>To determine if a quotas are supported, check the 
		/// <see cref="IMailStore.SupportsQuotas"/> property.</para>
		/// </remarks>
		/// <returns>The updated folder quota.</returns>
		/// <param name="messageLimit">If not <c>null</c>, sets the maximum number of messages to allow.</param>
		/// <param name="storageLimit">If not <c>null</c>, sets the maximum storage size (in kilobytes).</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		FolderQuota SetQuota (uint? messageLimit, uint? storageLimit, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously set the quota limits for the folder.
		/// </summary>
		/// <remarks>
		/// <para>Asynchronously sets the quota limits for the folder.</para>
		/// <para>To determine if a quotas are supported, check the 
		/// <see cref="IMailStore.SupportsQuotas"/> property.</para>
		/// </remarks>
		/// <returns>The updated folder quota.</returns>
		/// <param name="messageLimit">If not <c>null</c>, sets the maximum number of messages to allow.</param>
		/// <param name="storageLimit">If not <c>null</c>, sets the maximum storage size (in kilobytes).</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<FolderQuota> SetQuotaAsync (uint? messageLimit, uint? storageLimit, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets the specified metadata.
		/// </summary>
		/// <remarks>
		/// Gets the specified metadata.
		/// </remarks>
		/// <returns>The requested metadata value.</returns>
		/// <param name="tag">The metadata tag.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		string GetMetadata (MetadataTag tag, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously gets the specified metadata.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the specified metadata.
		/// </remarks>
		/// <returns>The requested metadata value.</returns>
		/// <param name="tag">The metadata tag.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<string> GetMetadataAsync (MetadataTag tag, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets the specified metadata.
		/// </summary>
		/// <remarks>
		/// Gets the specified metadata.
		/// </remarks>
		/// <returns>The requested metadata.</returns>
		/// <param name="tags">The metadata tags.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		MetadataCollection GetMetadata (IEnumerable<MetadataTag> tags, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously gets the specified metadata.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the specified metadata.
		/// </remarks>
		/// <returns>The requested metadata.</returns>
		/// <param name="tags">The metadata tags.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<MetadataCollection> GetMetadataAsync (IEnumerable<MetadataTag> tags, CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets the specified metadata.
		/// </summary>
		/// <remarks>
		/// Gets the specified metadata.
		/// </remarks>
		/// <returns>The requested metadata.</returns>
		/// <param name="options">The metadata options.</param>
		/// <param name="tags">The metadata tags.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		MetadataCollection GetMetadata (MetadataOptions options, IEnumerable<MetadataTag> tags, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously gets the specified metadata.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the specified metadata.
		/// </remarks>
		/// <returns>The requested metadata.</returns>
		/// <param name="options">The metadata options.</param>
		/// <param name="tags">The metadata tags.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<MetadataCollection> GetMetadataAsync (MetadataOptions options, IEnumerable<MetadataTag> tags, CancellationToken cancellationToken = default);

		/// <summary>
		/// Sets the specified metadata.
		/// </summary>
		/// <remarks>
		/// Sets the specified metadata.
		/// </remarks>
		/// <param name="metadata">The metadata.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		void SetMetadata (MetadataCollection metadata, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously sets the specified metadata.
		/// </summary>
		/// <remarks>
		/// Asynchronously sets the specified metadata.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="metadata">The metadata.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task SetMetadataAsync (MetadataCollection metadata, CancellationToken cancellationToken = default);

		/// <summary>
		/// Expunge the folder, permanently removing all messages marked for deletion.
		/// </summary>
		/// <remarks>
		/// <para>Expunges the folder, permanently removing all messages marked for deletion.</para>
		/// <note type="note">Normally, an <see cref="MessageExpunged"/> event will be emitted for each
		/// message that is expunged. However, if the mail store supports the quick
		/// resynchronization feature and it has been enabled via the
		/// <see cref="IMailStore.EnableQuickResync(CancellationToken)"/> method, then
		/// the <see cref="MessagesVanished"/> event will be emitted rather than the
		/// <see cref="MessageExpunged"/> event.</note>
		/// </remarks>
		/// <param name="cancellationToken">The cancellation token.</param>
		void Expunge (CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously expunge the folder, permanently removing all messages marked for deletion.
		/// </summary>
		/// <remarks>
		/// <para>Asynchronously expunges the folder, permanently removing all messages marked for deletion.</para>
		/// <note type="note">Normally, an <see cref="MessageExpunged"/> event will be emitted for
		/// each message that is expunged. However, if the mail store supports the quick
		/// resynchronization feature and it has been enabled via the
		/// <see cref="IMailStore.EnableQuickResync(CancellationToken)"/> method, then
		/// the <see cref="MessagesVanished"/> event will be emitted rather than the
		/// <see cref="MessageExpunged"/> event.</note>
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task ExpungeAsync (CancellationToken cancellationToken = default);

		/// <summary>
		/// Expunge the specified uids, permanently removing them from the folder.
		/// </summary>
		/// <remarks>
		/// <para>Expunges the specified uids, permanently removing them from the folder.</para>
		/// <note type="note">Normally, an <see cref="MessageExpunged"/> event will be emitted for
		/// each message that is expunged. However, if the mail store supports the quick
		/// resynchronization feature and it has been enabled via the
		/// <see cref="IMailStore.EnableQuickResync(CancellationToken)"/> method, then
		/// the <see cref="MessagesVanished"/> event will be emitted rather than the
		/// <see cref="MessageExpunged"/> event.</note>
		/// </remarks>
		/// <param name="uids">The message uids.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		void Expunge (IList<UniqueId> uids, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously expunge the specified uids, permanently removing them from the folder.
		/// </summary>
		/// <remarks>
		/// <para>Asynchronously expunges the specified uids, permanently removing them from the folder.</para>
		/// <note type="note">Normally, an <see cref="MessageExpunged"/> event will be emitted for
		/// each message that is expunged. However, if the mail store supports the quick
		/// resynchronization feature and it has been enabled via the
		/// <see cref="IMailStore.EnableQuickResync(CancellationToken)"/> method, then
		/// the <see cref="MessagesVanished"/> event will be emitted rather than the
		/// <see cref="MessageExpunged"/> event.</note>
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="uids">The message uids.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task ExpungeAsync (IList<UniqueId> uids, CancellationToken cancellationToken = default);

		/// <summary>
		/// Append a message to the folder.
		/// </summary>
		/// <remarks>
		/// Appends a message to the folder and returns the UniqueId assigned to the message.
		/// </remarks>
		/// <returns>The UID of the appended message, if available; otherwise, <c>null</c>.</returns>
		/// <param name="request">The append request.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		UniqueId? Append (IAppendRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously append a message to the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously appends a message to the folder and returns the UniqueId assigned to the message.
		/// </remarks>
		/// <returns>The UID of the appended message, if available; otherwise, <c>null</c>.</returns>
		/// <param name="request">The append request.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<UniqueId?> AppendAsync (IAppendRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Append a message to the folder.
		/// </summary>
		/// <remarks>
		/// Appends a message to the folder and returns the UniqueId assigned to the message.
		/// </remarks>
		/// <returns>The UID of the appended message, if available; otherwise, <c>null</c>.</returns>
		/// <param name="options">The formatting options.</param>
		/// <param name="request">The append request.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		UniqueId? Append (FormatOptions options, IAppendRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously append a message to the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously appends a message to the folder and returns the UniqueId assigned to the message.
		/// </remarks>
		/// <returns>The UID of the appended message, if available; otherwise, <c>null</c>.</returns>
		/// <param name="options">The formatting options.</param>
		/// <param name="request">The append request.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<UniqueId?> AppendAsync (FormatOptions options, IAppendRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Append multiple messages to the folder.
		/// </summary>
		/// <remarks>
		/// Appends multiple messages to the folder and returns the UniqueIds assigned to each of the messages.
		/// </remarks>
		/// <returns>The UIDs of the appended messages, if available; otherwise, an empty array.</returns>
		/// <param name="requests">The append requests.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IList<UniqueId> Append (IList<IAppendRequest> requests, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously append multiple messages to the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously appends multiple messages to the folder and returns the UniqueIds assigned to each of the messages.
		/// </remarks>
		/// <returns>The UID of the appended message, if available; otherwise, an empty array.</returns>
		/// <param name="requests">The append requests.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IList<UniqueId>> AppendAsync (IList<IAppendRequest> requests, CancellationToken cancellationToken = default);

		/// <summary>
		/// Append multiple messages to the folder.
		/// </summary>
		/// <remarks>
		/// Appends multiple messages to the folder and returns the UniqueIds assigned to each of the messages.
		/// </remarks>
		/// <returns>The UIDs of the appended messages, if available; otherwise, an empty array.</returns>
		/// <param name="options">The formatting options.</param>
		/// <param name="requests">The append requests.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IList<UniqueId> Append (FormatOptions options, IList<IAppendRequest> requests, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously append multiple messages to the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously appends multiple messages to the folder and returns the UniqueIds assigned to each of the messages.
		/// </remarks>
		/// <returns>The UID of the appended message, if available; otherwise, an empty array.</returns>
		/// <param name="options">The formatting options.</param>
		/// <param name="requests">The append requests.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IList<UniqueId>> AppendAsync (FormatOptions options, IList<IAppendRequest> requests, CancellationToken cancellationToken = default);

		/// <summary>
		/// Replace a message in the folder.
		/// </summary>
		/// <remarks>
		/// Replaces a message in the folder and returns the UniqueId assigned to the new message.
		/// </remarks>
		/// <returns>The UID of the new message, if available; otherwise, <c>null</c>.</returns>
		/// <param name="uid">The UID of the message to be replaced.</param>
		/// <param name="request">The replace request.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		UniqueId? Replace (UniqueId uid, IReplaceRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously replace a message in the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously replaces a message in the folder and returns the UniqueId assigned to the new message.
		/// </remarks>
		/// <returns>The UID of the appended message, if available; otherwise, <c>null</c>.</returns>
		/// <param name="uid">The UID of the message to be replaced.</param>
		/// <param name="request">The replace request.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<UniqueId?> ReplaceAsync (UniqueId uid, IReplaceRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Replace a message in the folder.
		/// </summary>
		/// <remarks>
		/// Replaces a message in the folder and returns the UniqueId assigned to the new message.
		/// </remarks>
		/// <returns>The UID of the new message, if available; otherwise, <c>null</c>.</returns>
		/// <param name="options">The formatting options.</param>
		/// <param name="uid">The UID of the message to be replaced.</param>
		/// <param name="request">The replace request.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		UniqueId? Replace (FormatOptions options, UniqueId uid, IReplaceRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously replace a message in the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously replaces a message in the folder and returns the UniqueId assigned to the new message.
		/// </remarks>
		/// <returns>The UID of the new message, if available; otherwise, <c>null</c>.</returns>
		/// <param name="options">The formatting options.</param>
		/// <param name="uid">The UID of the message to be replaced.</param>
		/// <param name="request">The replace request.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<UniqueId?> ReplaceAsync (FormatOptions options, UniqueId uid, IReplaceRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Replace a message in the folder.
		/// </summary>
		/// <remarks>
		/// Replaces a message in the folder and returns the UniqueId assigned to the new message.
		/// </remarks>
		/// <returns>The UID of the new message, if available; otherwise, <c>null</c>.</returns>
		/// <param name="index">The index of the message to be replaced.</param>
		/// <param name="request">The replace request.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		UniqueId? Replace (int index, IReplaceRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously replace a message in the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously replaces a message in the folder and returns the UniqueId assigned to the new message.
		/// </remarks>
		/// <returns>The UID of the appended message, if available; otherwise, <c>null</c>.</returns>
		/// <param name="index">The index of the message to be replaced.</param>
		/// <param name="request">The replace request.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<UniqueId?> ReplaceAsync (int index, IReplaceRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Replace a message in the folder.
		/// </summary>
		/// <remarks>
		/// Replaces a message in the folder and returns the UniqueId assigned to the new message.
		/// </remarks>
		/// <returns>The UID of the new message, if available; otherwise, <c>null</c>.</returns>
		/// <param name="options">The formatting options.</param>
		/// <param name="index">The index of the message to be replaced.</param>
		/// <param name="request">The replace request.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		UniqueId? Replace (FormatOptions options, int index, IReplaceRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously replace a message in the folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously replaces a message in the folder and returns the UniqueId assigned to the new message.
		/// </remarks>
		/// <returns>The UID of the new message, if available; otherwise, <c>null</c>.</returns>
		/// <param name="options">The formatting options.</param>
		/// <param name="index">The index of the message to be replaced.</param>
		/// <param name="request">The replace request.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<UniqueId?> ReplaceAsync (FormatOptions options, int index, IReplaceRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Copy the specified message to the destination folder.
		/// </summary>
		/// <remarks>
		/// Copies the specified message to the destination folder.
		/// </remarks>
		/// <returns>The UID of the message in the destination folder, if available; otherwise, <c>null</c>.</returns>
		/// <param name="uid">The UID of the message to copy.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		UniqueId? CopyTo (UniqueId uid, IMailFolder destination, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously copy the specified message to the destination folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously copies the specified message to the destination folder.
		/// </remarks>
		/// <returns>The UID of the message in the destination folder, if available; otherwise, <c>null</c>.</returns>
		/// <param name="uid">The UID of the message to copy.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<UniqueId?> CopyToAsync (UniqueId uid, IMailFolder destination, CancellationToken cancellationToken = default);

		/// <summary>
		/// Copy the specified messages to the destination folder.
		/// </summary>
		/// <remarks>
		/// Copies the specified messages to the destination folder.
		/// </remarks>
		/// <returns>The UID mapping of the messages in the destination folder, if available; otherwise an empty mapping.</returns>
		/// <param name="uids">The UIDs of the messages to copy.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		UniqueIdMap CopyTo (IList<UniqueId> uids, IMailFolder destination, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously copy the specified messages to the destination folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously copies the specified messages to the destination folder.
		/// </remarks>
		/// <returns>The UID mapping of the messages in the destination folder, if available; otherwise an empty mapping.</returns>
		/// <param name="uids">The UIDs of the messages to copy.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<UniqueIdMap> CopyToAsync (IList<UniqueId> uids, IMailFolder destination, CancellationToken cancellationToken = default);

		/// <summary>
		/// Move the specified message to the destination folder.
		/// </summary>
		/// <remarks>
		/// Moves the specified message to the destination folder.
		/// </remarks>
		/// <returns>The UID of the message in the destination folder, if available; otherwise, <c>null</c>.</returns>
		/// <param name="uid">The UID of the message to move.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		UniqueId? MoveTo (UniqueId uid, IMailFolder destination, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously move the specified message to the destination folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously moves the specified message to the destination folder.
		/// </remarks>
		/// <returns>The UID of the message in the destination folder, if available; otherwise, <c>null</c>.</returns>
		/// <param name="uid">The UID of the message to move.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<UniqueId?> MoveToAsync (UniqueId uid, IMailFolder destination, CancellationToken cancellationToken = default);

		/// <summary>
		/// Move the specified messages to the destination folder.
		/// </summary>
		/// <remarks>
		/// Moves the specified messages to the destination folder.
		/// </remarks>
		/// <returns>The UID mapping of the messages in the destination folder, if available; otherwise an empty mapping.</returns>
		/// <param name="uids">The UIDs of the messages to copy.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		UniqueIdMap MoveTo (IList<UniqueId> uids, IMailFolder destination, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously move the specified messages to the destination folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously moves the specified messages to the destination folder.
		/// </remarks>
		/// <returns>The UID mapping of the messages in the destination folder, if available; otherwise an empty mapping.</returns>
		/// <param name="uids">The UIDs of the messages to copy.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<UniqueIdMap> MoveToAsync (IList<UniqueId> uids, IMailFolder destination, CancellationToken cancellationToken = default);

		/// <summary>
		/// Copy the specified message to the destination folder.
		/// </summary>
		/// <remarks>
		/// Copies the specified message to the destination folder.
		/// </remarks>
		/// <param name="index">The index of the message to copy.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		void CopyTo (int index, IMailFolder destination, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously copy the specified message to the destination folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously copies the specified message to the destination folder.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="index">The indexes of the message to copy.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task CopyToAsync (int index, IMailFolder destination, CancellationToken cancellationToken = default);

		/// <summary>
		/// Copy the specified messages to the destination folder.
		/// </summary>
		/// <remarks>
		/// Copies the specified messages to the destination folder.
		/// </remarks>
		/// <param name="indexes">The indexes of the messages to copy.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		void CopyTo (IList<int> indexes, IMailFolder destination, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously copy the specified messages to the destination folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously copies the specified messages to the destination folder.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="indexes">The indexes of the messages to copy.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task CopyToAsync (IList<int> indexes, IMailFolder destination, CancellationToken cancellationToken = default);

		/// <summary>
		/// Move the specified message to the destination folder.
		/// </summary>
		/// <remarks>
		/// Moves the specified message to the destination folder.
		/// </remarks>
		/// <param name="index">The index of the message to move.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		void MoveTo (int index, IMailFolder destination, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously move the specified message to the destination folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously moves the specified message to the destination folder.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="index">The index of the message to move.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task MoveToAsync (int index, IMailFolder destination, CancellationToken cancellationToken = default);

		/// <summary>
		/// Move the specified messages to the destination folder.
		/// </summary>
		/// <remarks>
		/// Moves the specified messages to the destination folder.
		/// </remarks>
		/// <param name="indexes">The indexes of the messages to move.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		void MoveTo (IList<int> indexes, IMailFolder destination, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously move the specified messages to the destination folder.
		/// </summary>
		/// <remarks>
		/// Asynchronously moves the specified messages to the destination folder.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="indexes">The indexes of the messages to move.</param>
		/// <param name="destination">The destination folder.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task MoveToAsync (IList<int> indexes, IMailFolder destination, CancellationToken cancellationToken = default);

		/// <summary>
		/// Fetch the message summaries for the specified message UIDs.
		/// </summary>
		/// <remarks>
		/// <para>Fetches the message summaries for the specified message UIDs.</para>
		/// <para>It should be noted that if another client has modified any message
		/// in the folder, the mail service may choose to return information that was
		/// not explicitly requested. It is therefore important to be prepared to
		/// handle both additional fields on a <see cref="IMessageSummary"/> for
		/// messages that were requested as well as summaries for messages that were
		/// not requested at all.</para>
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\ImapBodyPartExamples.cs" region="GetBodyPartsByUniqueId"/>
		/// </example>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="uids">The UIDs.</param>
		/// <param name="request">The fetch request.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IList<IMessageSummary> Fetch (IList<UniqueId> uids, IFetchRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously fetch the message summaries for the specified message UIDs.
		/// </summary>
		/// <remarks>
		/// <para>Asynchronously fetches the message summaries for the specified message
		/// UIDs.</para>
		/// <para>It should be noted that if another client has modified any message
		/// in the folder, the mail service may choose to return information that was
		/// not explicitly requested. It is therefore important to be prepared to
		/// handle both additional fields on a <see cref="IMessageSummary"/> for
		/// messages that were requested as well as summaries for messages that were
		/// not requested at all.</para>
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="uids">The UIDs.</param>
		/// <param name="request">The fetch request.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IList<IMessageSummary>> FetchAsync (IList<UniqueId> uids, IFetchRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Fetch the message summaries for the specified message indexes.
		/// </summary>
		/// <remarks>
		/// <para>Fetches the message summaries for the specified message indexes.</para>
		/// <para>It should be noted that if another client has modified any message
		/// in the folder, the mail service may choose to return information that was
		/// not explicitly requested. It is therefore important to be prepared to
		/// handle both additional fields on a <see cref="IMessageSummary"/> for
		/// messages that were requested as well as summaries for messages that were
		/// not requested at all.</para>
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="indexes">The indexes.</param>
		/// <param name="request">The fetch request.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IList<IMessageSummary> Fetch (IList<int> indexes, IFetchRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously fetch the message summaries for the specified message indexes.
		/// </summary>
		/// <remarks>
		/// <para>Asynchronously fetches the message summaries for the specified message
		/// indexes.</para>
		/// <para>It should be noted that if another client has modified any message
		/// in the folder, the mail service may choose to return information that was
		/// not explicitly requested. It is therefore important to be prepared to
		/// handle both additional fields on a <see cref="IMessageSummary"/> for
		/// messages that were requested as well as summaries for messages that were
		/// not requested at all.</para>
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="indexes">The indexes.</param>
		/// <param name="request">The fetch request.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IList<IMessageSummary>> FetchAsync (IList<int> indexes, IFetchRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Fetch the message summaries for the messages between the two indexes, inclusive.
		/// </summary>
		/// <remarks>
		/// <para>Fetches the message summaries for the messages between the two
		/// indexes, inclusive.</para>
		/// <para>It should be noted that if another client has modified any message
		/// in the folder, the mail service may choose to return information that was
		/// not explicitly requested. It is therefore important to be prepared to
		/// handle both additional fields on a <see cref="IMessageSummary"/> for
		/// messages that were requested as well as summaries for messages that were
		/// not requested at all.</para>
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="min">The minimum index.</param>
		/// <param name="max">The maximum index, or <c>-1</c> to specify no upper bound.</param>
		/// <param name="request">The fetch request.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IList<IMessageSummary> Fetch (int min, int max, IFetchRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously fetch the message summaries for the messages between the two indexes, inclusive.
		/// </summary>
		/// <remarks>
		/// <para>Asynchronously fetches the message summaries for the messages between
		/// the two indexes, inclusive.</para>
		/// <para>It should be noted that if another client has modified any message
		/// in the folder, the mail service may choose to return information that was
		/// not explicitly requested. It is therefore important to be prepared to
		/// handle both additional fields on a <see cref="IMessageSummary"/> for
		/// messages that were requested as well as summaries for messages that were
		/// not requested at all.</para>
		/// </remarks>
		/// <returns>An enumeration of summaries for the requested messages.</returns>
		/// <param name="min">The minimum index.</param>
		/// <param name="max">The maximum index, or <c>-1</c> to specify no upper bound.</param>
		/// <param name="request">The fetch request.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IList<IMessageSummary>> FetchAsync (int min, int max, IFetchRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Get the specified message headers.
		/// </summary>
		/// <remarks>
		/// Gets the specified message headers.
		/// </remarks>
		/// <returns>The message headers.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		HeaderList GetHeaders (UniqueId uid, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Asynchronously get the specified message headers.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the specified message headers.
		/// </remarks>
		/// <returns>The message headers.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Task<HeaderList> GetHeadersAsync (UniqueId uid, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Get the specified body part headers.
		/// </summary>
		/// <remarks>
		/// Gets the specified body part headers.
		/// </remarks>
		/// <returns>The body part headers.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="part">The body part.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		HeaderList GetHeaders (UniqueId uid, BodyPart part, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Asynchronously get the specified body part headers.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the specified body part headers.
		/// </remarks>
		/// <returns>The body part headers.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="part">The body part.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Task<HeaderList> GetHeadersAsync (UniqueId uid, BodyPart part, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Get the specified message headers.
		/// </summary>
		/// <remarks>
		/// Gets the specified message headers.
		/// </remarks>
		/// <returns>The message headers.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		HeaderList GetHeaders (int index, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Asynchronously get the specified message headers.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the specified message headers.
		/// </remarks>
		/// <returns>The message headers.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Task<HeaderList> GetHeadersAsync (int index, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Get the specified body part headers.
		/// </summary>
		/// <remarks>
		/// Gets the specified body part headers.
		/// </remarks>
		/// <returns>The body part headers.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="part">The body part.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		HeaderList GetHeaders (int index, BodyPart part, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Asynchronously get the specified body part headers.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the specified body part headers.
		/// </remarks>
		/// <returns>The body part headers.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="part">The body part.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Task<HeaderList> GetHeadersAsync (int index, BodyPart part, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Get the specified message.
		/// </summary>
		/// <remarks>
		/// Gets the specified message.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\ImapExamples.cs" region="DownloadMessagesByUniqueId"/>
		/// </example>
		/// <returns>The message.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		MimeMessage GetMessage (UniqueId uid, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Asynchronously get the specified message.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the specified message.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\ImapExamples.cs" region="DownloadMessagesByUniqueId"/>
		/// </example>
		/// <returns>The message.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Task<MimeMessage> GetMessageAsync (UniqueId uid, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Get the specified message.
		/// </summary>
		/// <remarks>
		/// Gets the specified message.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\ImapExamples.cs" region="DownloadMessagesByIndex"/>
		/// </example>
		/// <returns>The message.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		MimeMessage GetMessage (int index, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Asynchronously get the specified message.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the specified message.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\ImapExamples.cs" region="DownloadMessagesByIndex"/>
		/// </example>
		/// <returns>The message.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Task<MimeMessage> GetMessageAsync (int index, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Get the specified body part.
		/// </summary>
		/// <remarks>
		/// Gets the specified body part.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\ImapBodyPartExamples.cs" region="GetBodyPartsByUniqueId"/>
		/// </example>
		/// <returns>The body part.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="part">The body part.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		MimeEntity GetBodyPart (UniqueId uid, BodyPart part, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Asynchronously get the specified body part.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the specified body part.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\ImapBodyPartExamples.cs" region="GetBodyPartsByUniqueId"/>
		/// </example>
		/// <returns>The body part.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="part">The body part.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Task<MimeEntity> GetBodyPartAsync (UniqueId uid, BodyPart part, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Get the specified body part.
		/// </summary>
		/// <remarks>
		/// Gets the specified body part.
		/// </remarks>
		/// <returns>The body part.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="part">The body part.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		MimeEntity GetBodyPart (int index, BodyPart part, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Asynchronously get the specified body part.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets the specified body part.
		/// </remarks>
		/// <returns>The body part.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="part">The body part.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Task<MimeEntity> GetBodyPartAsync (int index, BodyPart part, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Get a message stream.
		/// </summary>
		/// <remarks>
		/// Gets a message stream.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\ImapExamples.cs" region="DownloadMessageStreamsByUniqueId"/>
		/// </example>
		/// <returns>The message stream.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Stream GetStream (UniqueId uid, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Asynchronously get a message stream.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets a message stream.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\ImapExamples.cs" region="DownloadMessageStreamsByUniqueId"/>
		/// </example>
		/// <returns>The message stream.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Task<Stream> GetStreamAsync (UniqueId uid, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Get a message stream.
		/// </summary>
		/// <remarks>
		/// Gets a message stream.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\ImapExamples.cs" region="DownloadMessageStreamsByIndex"/>
		/// </example>
		/// <returns>The message stream.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Stream GetStream (int index, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Asynchronously get a message stream.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets a message stream.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\ImapExamples.cs" region="DownloadMessageStreamsByIndex"/>
		/// </example>
		/// <returns>The message stream.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Task<Stream> GetStreamAsync (int index, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Get a substream of the specified message.
		/// </summary>
		/// <remarks>
		/// Gets a substream of the message. If the starting offset is beyond
		/// the end of the message, an empty stream is returned. If the number of
		/// bytes desired extends beyond the end of the message, a truncated stream
		/// will be returned.
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="offset">The starting offset of the first desired byte.</param>
		/// <param name="count">The number of bytes desired.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Stream GetStream (UniqueId uid, int offset, int count, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Asynchronously get a substream of the specified message.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets a substream of the message. If the starting offset is beyond
		/// the end of the message, an empty stream is returned. If the number of
		/// bytes desired extends beyond the end of the message, a truncated stream
		/// will be returned.
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="offset">The starting offset of the first desired byte.</param>
		/// <param name="count">The number of bytes desired.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Task<Stream> GetStreamAsync (UniqueId uid, int offset, int count, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Get a substream of the specified message.
		/// </summary>
		/// <remarks>
		/// Gets a substream of the message. If the starting offset is beyond
		/// the end of the message, an empty stream is returned. If the number of
		/// bytes desired extends beyond the end of the message, a truncated stream
		/// will be returned.
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="offset">The starting offset of the first desired byte.</param>
		/// <param name="count">The number of bytes desired.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Stream GetStream (int index, int offset, int count, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Asynchronously get a substream of the specified message.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets a substream of the message. If the starting offset is beyond
		/// the end of the message, an empty stream is returned. If the number of
		/// bytes desired extends beyond the end of the message, a truncated stream
		/// will be returned.
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="offset">The starting offset of the first desired byte.</param>
		/// <param name="count">The number of bytes desired.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Task<Stream> GetStreamAsync (int index, int offset, int count, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Get a body part as a stream.
		/// </summary>
		/// <remarks>
		/// Gets a body part as a stream.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\ImapBodyPartExamples.cs" region="GetBodyPartStreamsByUniqueId"/>
		/// </example>
		/// <returns>The body part stream.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="part">The desired body part.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Stream GetStream (UniqueId uid, BodyPart part, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Asynchronously get a body part as a stream.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets a body part as a stream.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\ImapBodyPartExamples.cs" region="GetBodyPartStreamsByUniqueId"/>
		/// </example>
		/// <returns>The body part stream.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="part">The desired body part.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Task<Stream> GetStreamAsync (UniqueId uid, BodyPart part, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Get a body part as a stream.
		/// </summary>
		/// <remarks>
		/// Gets a body part as a stream.
		/// </remarks>
		/// <returns>The body part stream.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="part">The desired body part.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Stream GetStream (int index, BodyPart part, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Asynchronously get a body part as a stream.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets a body part as a stream.
		/// </remarks>
		/// <returns>The body part stream.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="part">The desired body part.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Task<Stream> GetStreamAsync (int index, BodyPart part, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Get a substream of the specified body part.
		/// </summary>
		/// <remarks>
		/// Gets a substream of the body part. If the starting offset is beyond
		/// the end of the body part, an empty stream is returned. If the number of
		/// bytes desired extends beyond the end of the body part, a truncated stream
		/// will be returned.
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="part">The desired body part.</param>
		/// <param name="offset">The starting offset of the first desired byte.</param>
		/// <param name="count">The number of bytes desired.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Stream GetStream (UniqueId uid, BodyPart part, int offset, int count, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Asynchronously get a substream of the specified body part.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets a substream of the body part. If the starting offset is beyond
		/// the end of the body part, an empty stream is returned. If the number of
		/// bytes desired extends beyond the end of the body part, a truncated stream
		/// will be returned.
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="part">The desired body part.</param>
		/// <param name="offset">The starting offset of the first desired byte.</param>
		/// <param name="count">The number of bytes desired.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Task<Stream> GetStreamAsync (UniqueId uid, BodyPart part, int offset, int count, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Get a substream of the specified body part.
		/// </summary>
		/// <remarks>
		/// Gets a substream of the body part. If the starting offset is beyond
		/// the end of the body part, an empty stream is returned. If the number of
		/// bytes desired extends beyond the end of the body part, a truncated stream
		/// will be returned.
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="part">The desired body part.</param>
		/// <param name="offset">The starting offset of the first desired byte.</param>
		/// <param name="count">The number of bytes desired.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Stream GetStream (int index, BodyPart part, int offset, int count, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Asynchronously get a substream of the specified body part.
		/// </summary>
		/// <remarks>
		/// Asynchronously gets a substream of the body part. If the starting offset is beyond
		/// the end of the body part, an empty stream is returned. If the number of
		/// bytes desired extends beyond the end of the body part, a truncated stream
		/// will be returned.
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="part">The desired body part.</param>
		/// <param name="offset">The starting offset of the first desired byte.</param>
		/// <param name="count">The number of bytes desired.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Task<Stream> GetStreamAsync (int index, BodyPart part, int offset, int count, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Get a substream of the specified message.
		/// </summary>
		/// <remarks>
		/// <para>Gets a substream of the specified message.</para>
		/// <para>For more information about how to construct the <paramref name="section"/>,
		/// see Section 6.4.5 of RFC3501.</para>
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\ImapBodyPartExamples.cs" region="GetBodyPartStreamsByUniqueIdAndSpecifier"/>
		/// </example>
		/// <returns>The stream.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="section">The desired section of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Stream GetStream (UniqueId uid, string section, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Asynchronously get a substream of the specified message.
		/// </summary>
		/// <remarks>
		/// <para>Asynchronously gets a substream of the specified message.</para>
		/// <para>For more information about how to construct the <paramref name="section"/>,
		/// see Section 6.4.5 of RFC3501.</para>
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\ImapBodyPartExamples.cs" region="GetBodyPartStreamsByUniqueIdAndSpecifier"/>
		/// </example>
		/// <returns>The stream.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="section">The desired section of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Task<Stream> GetStreamAsync (UniqueId uid, string section, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Get a substream of the specified message.
		/// </summary>
		/// <remarks>
		/// <para>Gets a substream of the specified message. If the starting offset is beyond
		/// the end of the specified section of the message, an empty stream is returned. If
		/// the number of bytes desired extends beyond the end of the section, a truncated
		/// stream will be returned.</para>
		/// <para>For more information about how to construct the <paramref name="section"/>,
		/// see Section 6.4.5 of RFC3501.</para>
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="section">The desired section of the message.</param>
		/// <param name="offset">The starting offset of the first desired byte.</param>
		/// <param name="count">The number of bytes desired.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Stream GetStream (UniqueId uid, string section, int offset, int count, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Asynchronously get a substream of the specified message.
		/// </summary>
		/// <remarks>
		/// <para>Asynchronously gets a substream of the specified message. If the starting
		/// offset is beyond the end of the specified section of the message, an empty stream
		/// is returned. If the number of bytes desired extends beyond the end of the section,
		/// a truncated stream will be returned.</para>
		/// <para>For more information about how to construct the <paramref name="section"/>,
		/// see Section 6.4.5 of RFC3501.</para>
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="section">The desired section of the message.</param>
		/// <param name="offset">The starting offset of the first desired byte.</param>
		/// <param name="count">The number of bytes desired.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Task<Stream> GetStreamAsync (UniqueId uid, string section, int offset, int count, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Get a substream of the specified message.
		/// </summary>
		/// <remarks>
		/// <para>Gets a substream of the specified message.</para>
		/// <para>For more information about how to construct the <paramref name="section"/>,
		/// see Section 6.4.5 of RFC3501.</para>
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="section">The desired section of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Stream GetStream (int index, string section, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Asynchronously get a substream of the specified message.
		/// </summary>
		/// <remarks>
		/// <para>Asynchronously gets a substream of the specified message.</para>
		/// <para>For more information about how to construct the <paramref name="section"/>,
		/// see Section 6.4.5 of RFC3501.</para>
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="section">The desired section of the message.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Task<Stream> GetStreamAsync (int index, string section, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Get a substream of the specified message.
		/// </summary>
		/// <remarks>
		/// <para>Gets a substream of the specified message. If the starting offset is beyond
		/// the end of the specified section of the message, an empty stream is returned. If
		/// the number of bytes desired extends beyond the end of the section, a truncated
		/// stream will be returned.</para>
		/// <para>For more information about how to construct the <paramref name="section"/>,
		/// see Section 6.4.5 of RFC3501.</para>
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="section">The desired section of the message.</param>
		/// <param name="offset">The starting offset of the first desired byte.</param>
		/// <param name="count">The number of bytes desired.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Stream GetStream (int index, string section, int offset, int count, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Asynchronously get a substream of the specified message.
		/// </summary>
		/// <remarks>
		/// <para>Asynchronously gets a substream of the specified message. If the starting
		/// offset is beyond the end of the specified section of the message, an empty stream
		/// is returned. If the number of bytes desired extends beyond the end of the section,
		/// a truncated stream will be returned.</para>
		/// <para>For more information about how to construct the <paramref name="section"/>,
		/// see Section 6.4.5 of RFC3501.</para>
		/// </remarks>
		/// <returns>The stream.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="section">The desired section of the message.</param>
		/// <param name="offset">The starting offset of the first desired byte.</param>
		/// <param name="count">The number of bytes desired.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <param name="progress">The progress reporting mechanism.</param>
		Task<Stream> GetStreamAsync (int index, string section, int offset, int count, CancellationToken cancellationToken = default, ITransferProgress progress = null);

		/// <summary>
		/// Store message flags and keywords for a message.
		/// </summary>
		/// <remarks>
		/// Updates the message flags and keywords for a message.
		/// </remarks>
		/// <returns><c>true</c> if the store operation was successful; otherwise, <c>false</c>.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="request">The message flags and keywords to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		bool Store (UniqueId uid, IStoreFlagsRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously store message flags and keywords for a message.
		/// </summary>
		/// <remarks>
		/// Asynchronously updates the message flags and keywords for a message.
		/// </remarks>
		/// <returns><c>true</c> if the store operation was successful; otherwise, <c>false</c>.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="request">The message flags and keywords to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<bool> StoreAsync (UniqueId uid, IStoreFlagsRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Store message flags and keywords for a set of messages.
		/// </summary>
		/// <remarks>
		/// Updates the message flags and keywords for a set of messages.
		/// </remarks>
		/// <returns>The UIDs of the messages that were not updated.</returns>
		/// <param name="uids">The message UIDs.</param>
		/// <param name="request">The message flags and keywords to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IList<UniqueId> Store (IList<UniqueId> uids, IStoreFlagsRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously store message flags and keywords for a set of messages.
		/// </summary>
		/// <remarks>
		/// Asynchronously updates the message flags and keywords for a set of messages.
		/// </remarks>
		/// <returns>The UIDs of the messages that were not updated.</returns>
		/// <param name="uids">The message UIDs.</param>
		/// <param name="request">The message flags and keywords to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IList<UniqueId>> StoreAsync (IList<UniqueId> uids, IStoreFlagsRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Store message flags and keywords for a message.
		/// </summary>
		/// <remarks>
		/// Updates the message flags and keywords for a message.
		/// </remarks>
		/// <returns><c>true</c> if the store operation was successful; otherwise, <c>false</c>.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="request">The message flags and keywords to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		bool Store (int index, IStoreFlagsRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously store message flags and keywords for a message.
		/// </summary>
		/// <remarks>
		/// Asynchronously updates the message flags and keywords for a message.
		/// </remarks>
		/// <returns><c>true</c> if the store operation was successful; otherwise, <c>false</c>.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="request">The message flags and keywords to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<bool> StoreAsync (int index, IStoreFlagsRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Store message flags and keywords for a set of messages.
		/// </summary>
		/// <remarks>
		/// Updates the message flags and keywords for a set of messages.
		/// </remarks>
		/// <returns>The indexes of the messages that were not updated.</returns>
		/// <param name="indexes">The message indexes.</param>
		/// <param name="request">The message flags and keywords to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IList<int> Store (IList<int> indexes, IStoreFlagsRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously store message flags and keywords for a set of messages.
		/// </summary>
		/// <remarks>
		/// Asynchronously updates the message flags and keywords for a set of message.
		/// </remarks>
		/// <returns>The indexes of the messages that were not updated.</returns>
		/// <param name="indexes">The message indexes.</param>
		/// <param name="request">The message flags and keywords to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IList<int>> StoreAsync (IList<int> indexes, IStoreFlagsRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Store GMail-style labels for a message.
		/// </summary>
		/// <remarks>
		/// Updates the GMail-style labels for a message.
		/// </remarks>
		/// <returns><c>true</c> if the store operation was successful; otherwise, <c>false</c>.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="request">The labels to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		bool Store (UniqueId uid, IStoreLabelsRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously store GMail-style labels for a message.
		/// </summary>
		/// <remarks>
		/// Asynchronously updates the GMail-style labels for a message.
		/// </remarks>
		/// <returns><c>true</c> if the store operation was successful; otherwise, <c>false</c>.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="request">The labels to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<bool> StoreAsync (UniqueId uid, IStoreLabelsRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Store GMail-style labels for a set of messages.
		/// </summary>
		/// <remarks>
		/// Updates the GMail-style labels for a set of messages.
		/// </remarks>
		/// <returns>The UIDs of the messages that were not updated.</returns>
		/// <param name="uids">The message UIDs.</param>
		/// <param name="request">The labels to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IList<UniqueId> Store (IList<UniqueId> uids, IStoreLabelsRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously store GMail-style labels for a set of messages.
		/// </summary>
		/// <remarks>
		/// Asynchronously updates the GMail-style labels for a set of messages.
		/// </remarks>
		/// <returns>The UIDs of the messages that were not updated.</returns>
		/// <param name="uids">The message UIDs.</param>
		/// <param name="request">The labels to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IList<UniqueId>> StoreAsync (IList<UniqueId> uids, IStoreLabelsRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Store GMail-style labels for a message.
		/// </summary>
		/// <remarks>
		/// Updates the GMail-style labels for a message.
		/// </remarks>
		/// <returns><c>true</c> if the store operation was successful; otherwise, <c>false</c>.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="request">The labels to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		bool Store (int index, IStoreLabelsRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously store GMail-style labels for a message.
		/// </summary>
		/// <remarks>
		/// Asynchronously updates the GMail-style labels for a message.
		/// </remarks>
		/// <returns><c>true</c> if the store operation was successful; otherwise, <c>false</c>.</returns>
		/// <param name="index">The index of the message.</param>
		/// <param name="request">The labels to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<bool> StoreAsync (int index, IStoreLabelsRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Store GMail-style labels for a set of messages.
		/// </summary>
		/// <remarks>
		/// Updates the GMail-style labels for a set of messages.
		/// </remarks>
		/// <returns>The indexes of the messages that were not updated.</returns>
		/// <param name="indexes">The message indexes.</param>
		/// <param name="request">The labels to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IList<int> Store (IList<int> indexes, IStoreLabelsRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously store GMail-style labels for a set of messages.
		/// </summary>
		/// <remarks>
		/// Asynchronously updates the GMail-style labels for a set of message.
		/// </remarks>
		/// <returns>The indexes of the messages that were not updated.</returns>
		/// <param name="indexes">The message indexes.</param>
		/// <param name="request">The labels to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IList<int>> StoreAsync (IList<int> indexes, IStoreLabelsRequest request, CancellationToken cancellationToken = default);

		/// <summary>
		/// Store the annotations for the specified message.
		/// </summary>
		/// <remarks>
		/// Stores the annotations for the specified message.
		/// </remarks>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="annotations">The annotations to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		void Store (UniqueId uid, IList<Annotation> annotations, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously store the annotations for the specified message.
		/// </summary>
		/// <remarks>
		/// Asynchronously stores the annotations for the specified message.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="uid">The UID of the message.</param>
		/// <param name="annotations">The annotations to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task StoreAsync (UniqueId uid, IList<Annotation> annotations, CancellationToken cancellationToken = default);

		/// <summary>
		/// Store the annotations for the specified messages.
		/// </summary>
		/// <remarks>
		/// Stores the annotations for the specified messages.
		/// </remarks>
		/// <param name="uids">The UIDs of the messages.</param>
		/// <param name="annotations">The annotations to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		void Store (IList<UniqueId> uids, IList<Annotation> annotations, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously store the annotations for the specified messages.
		/// </summary>
		/// <remarks>
		/// Asynchronously stores the annotations for the specified messages.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="uids">The UIDs of the messages.</param>
		/// <param name="annotations">The annotations to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task StoreAsync (IList<UniqueId> uids, IList<Annotation> annotations, CancellationToken cancellationToken = default);

		/// <summary>
		/// Store the annotations for the specified messages only if their mod-sequence value is less than the specified value.
		/// </summary>
		/// <remarks>
		/// Stores the annotations for the specified messages only if their mod-sequence value is less than the specified value.
		/// </remarks>
		/// <returns>The unique IDs of the messages that were not updated.</returns>
		/// <param name="uids">The UIDs of the messages.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="annotations">The annotations to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IList<UniqueId> Store (IList<UniqueId> uids, ulong modseq, IList<Annotation> annotations, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously store the annotations for the specified messages only if their mod-sequence value is less than the specified value.
		/// </summary>
		/// <remarks>
		/// Asynchronously stores the annotations for the specified messages only if their mod-sequence value is less than the specified value.
		/// </remarks>
		/// <returns>The unique IDs of the messages that were not updated.</returns>
		/// <param name="uids">The UIDs of the messages.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="annotations">The annotations to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IList<UniqueId>> StoreAsync (IList<UniqueId> uids, ulong modseq, IList<Annotation> annotations, CancellationToken cancellationToken = default);

		/// <summary>
		/// Store the annotations for the specified message.
		/// </summary>
		/// <remarks>
		/// Stores the annotations for the specified message.
		/// </remarks>
		/// <param name="index">The index of the message.</param>
		/// <param name="annotations">The annotations to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		void Store (int index, IList<Annotation> annotations, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously store the annotations for the specified message.
		/// </summary>
		/// <remarks>
		/// Asynchronously stores the annotations for the specified message.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="index">The indexes of the message.</param>
		/// <param name="annotations">The annotations to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task StoreAsync (int index, IList<Annotation> annotations, CancellationToken cancellationToken = default);

		/// <summary>
		/// Store the annotations for the specified messages.
		/// </summary>
		/// <remarks>
		/// Stores the annotations for the specified messages.
		/// </remarks>
		/// <param name="indexes">The indexes of the messages.</param>
		/// <param name="annotations">The annotations to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		void Store (IList<int> indexes, IList<Annotation> annotations, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously store the annotations for the specified messages.
		/// </summary>
		/// <remarks>
		/// Asynchronously stores the annotations for the specified messages.
		/// </remarks>
		/// <returns>An asynchronous task context.</returns>
		/// <param name="indexes">The indexes of the messages.</param>
		/// <param name="annotations">The annotations to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task StoreAsync (IList<int> indexes, IList<Annotation> annotations, CancellationToken cancellationToken = default);

		/// <summary>
		/// Store the annotations for the specified messages only if their mod-sequence value is less than the specified value.
		/// </summary>
		/// <remarks>
		/// Stores the annotations for the specified messages only if their mod-sequence value is less than the specified value.
		/// </remarks>
		/// <returns>The indexes of the messages that were not updated.</returns>
		/// <param name="indexes">The indexes of the messages.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="annotations">The annotations to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IList<int> Store (IList<int> indexes, ulong modseq, IList<Annotation> annotations, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously store the annotations for the specified messages only if their mod-sequence value is less than the specified value.
		/// </summary>
		/// <remarks>
		/// Asynchronously stores the annotations for the specified messages only if their mod-sequence value is less than the specified value.s
		/// </remarks>
		/// <returns>The indexes of the messages that were not updated.</returns>
		/// <param name="indexes">The indexes of the messages.</param>
		/// <param name="modseq">The mod-sequence value.</param>
		/// <param name="annotations">The annotations to store.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IList<int>> StoreAsync (IList<int> indexes, ulong modseq, IList<Annotation> annotations, CancellationToken cancellationToken = default);

		/// <summary>
		/// Search the folder for messages matching the specified query.
		/// </summary>
		/// <remarks>
		/// The returned array of unique identifiers can be used with methods such as
		/// <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken,ITransferProgress)"/>.
		/// </remarks>
		/// <returns>An array of matching UIDs.</returns>
		/// <param name="query">The search query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IList<UniqueId> Search (SearchQuery query, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously search the folder for messages matching the specified query.
		/// </summary>
		/// <remarks>
		/// The returned array of unique identifiers can be used with methods such as
		/// <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken,ITransferProgress)"/>.
		/// </remarks>
		/// <returns>An array of matching UIDs.</returns>
		/// <param name="query">The search query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IList<UniqueId>> SearchAsync (SearchQuery query, CancellationToken cancellationToken = default);

		/// <summary>
		/// Search the subset of UIDs in the folder for messages matching the specified query.
		/// </summary>
		/// <remarks>
		/// The returned array of unique identifiers can be used with methods such as
		/// <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken,ITransferProgress)"/>.
		/// </remarks>
		/// <returns>An array of matching UIDs.</returns>
		/// <param name="uids">The subset of UIDs</param>
		/// <param name="query">The search query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IList<UniqueId> Search (IList<UniqueId> uids, SearchQuery query, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously search the subset of UIDs in the folder for messages matching the specified query.
		/// </summary>
		/// <remarks>
		/// The returned array of unique identifiers can be used with methods such as
		/// <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken,ITransferProgress)"/>.
		/// </remarks>
		/// <returns>An array of matching UIDs.</returns>
		/// <param name="uids">The subset of UIDs</param>
		/// <param name="query">The search query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IList<UniqueId>> SearchAsync (IList<UniqueId> uids, SearchQuery query, CancellationToken cancellationToken = default);

		/// <summary>
		/// Search the folder for messages matching the specified query.
		/// </summary>
		/// <remarks>
		/// Searches the folder for messages matching the specified query,
		/// returning only the specified search results.
		/// </remarks>
		/// <returns>The search results.</returns>
		/// <param name="options">The search options.</param>
		/// <param name="query">The search query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		SearchResults Search (SearchOptions options, SearchQuery query, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously search the folder for messages matching the specified query.
		/// </summary>
		/// <remarks>
		/// Asynchronously searches the folder for messages matching the specified query,
		/// returning only the specified search results.
		/// </remarks>
		/// <returns>The search results.</returns>
		/// <param name="options">The search options.</param>
		/// <param name="query">The search query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<SearchResults> SearchAsync (SearchOptions options, SearchQuery query, CancellationToken cancellationToken = default);

		/// <summary>
		/// Search the subset of UIDs in the folder for messages matching the specified query.
		/// </summary>
		/// <remarks>
		/// Searches the fsubset of UIDs in the folder for messages matching the specified query,
		/// returning only the specified search results.
		/// </remarks>
		/// <returns>The search results.</returns>
		/// <param name="options">The search options.</param>
		/// <param name="uids">The subset of UIDs</param>
		/// <param name="query">The search query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		SearchResults Search (SearchOptions options, IList<UniqueId> uids, SearchQuery query, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously search the subset of UIDs in the folder for messages matching the specified query.
		/// </summary>
		/// <remarks>
		/// Asynchronously searches the fsubset of UIDs in the folder for messages matching the specified query,
		/// returning only the specified search results.
		/// </remarks>
		/// <returns>The search results.</returns>
		/// <param name="options">The search options.</param>
		/// <param name="uids">The subset of UIDs</param>
		/// <param name="query">The search query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<SearchResults> SearchAsync (SearchOptions options, IList<UniqueId> uids, SearchQuery query, CancellationToken cancellationToken = default);

		/// <summary>
		/// Sort messages matching the specified query.
		/// </summary>
		/// <remarks>
		/// The returned array of unique identifiers will be sorted in the preferred order and
		/// can be used with <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken,ITransferProgress)"/>.
		/// </remarks>
		/// <returns>An array of matching UIDs in the specified sort order.</returns>
		/// <param name="query">The search query.</param>
		/// <param name="orderBy">The sort order.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IList<UniqueId> Sort (SearchQuery query, IList<OrderBy> orderBy, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously sort messages matching the specified query.
		/// </summary>
		/// <remarks>
		/// The returned array of unique identifiers will be sorted in the preferred order and
		/// can be used with <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken,ITransferProgress)"/>.
		/// </remarks>
		/// <returns>An array of matching UIDs in the specified sort order.</returns>
		/// <param name="query">The search query.</param>
		/// <param name="orderBy">The sort order.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IList<UniqueId>> SortAsync (SearchQuery query, IList<OrderBy> orderBy, CancellationToken cancellationToken = default);

		/// <summary>
		/// Sort messages matching the specified query.
		/// </summary>
		/// <remarks>
		/// The returned array of unique identifiers will be sorted in the preferred order and
		/// can be used with <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken,ITransferProgress)"/>.
		/// </remarks>
		/// <returns>An array of matching UIDs in the specified sort order.</returns>
		/// <param name="uids">The subset of UIDs</param>
		/// <param name="query">The search query.</param>
		/// <param name="orderBy">The sort order.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IList<UniqueId> Sort (IList<UniqueId> uids, SearchQuery query, IList<OrderBy> orderBy, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously sort messages matching the specified query.
		/// </summary>
		/// <remarks>
		/// The returned array of unique identifiers will be sorted in the preferred order and
		/// can be used with <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken,ITransferProgress)"/>.
		/// </remarks>
		/// <returns>An array of matching UIDs in the specified sort order.</returns>
		/// <param name="uids">The subset of UIDs</param>
		/// <param name="query">The search query.</param>
		/// <param name="orderBy">The sort order.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IList<UniqueId>> SortAsync (IList<UniqueId> uids, SearchQuery query, IList<OrderBy> orderBy, CancellationToken cancellationToken = default);

		/// <summary>
		/// Sort messages matching the specified query.
		/// </summary>
		/// <remarks>
		/// Searches the folder for messages matching the specified query, returning the search results in the specified sort order.
		/// </remarks>
		/// <returns>The search results.</returns>
		/// <param name="options">The search options.</param>
		/// <param name="query">The search query.</param>
		/// <param name="orderBy">The sort order.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		SearchResults Sort (SearchOptions options, SearchQuery query, IList<OrderBy> orderBy, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously sort messages matching the specified query.
		/// </summary>
		/// <remarks>
		/// Asynchronously searches the folder for messages matching the specified query, returning the search results in the specified sort order.
		/// </remarks>
		/// <returns>The search results.</returns>
		/// <param name="options">The search options.</param>
		/// <param name="query">The search query.</param>
		/// <param name="orderBy">The sort order.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<SearchResults> SortAsync (SearchOptions options, SearchQuery query, IList<OrderBy> orderBy, CancellationToken cancellationToken = default);

		/// <summary>
		/// Sort messages matching the specified query.
		/// </summary>
		/// <remarks>
		/// Searches the folder for messages matching the specified query, returning the search results in the specified sort order.
		/// </remarks>
		/// <returns>The search results.</returns>
		/// <param name="options">The search options.</param>
		/// <param name="uids">The subset of UIDs</param>
		/// <param name="query">The search query.</param>
		/// <param name="orderBy">The sort order.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		SearchResults Sort (SearchOptions options, IList<UniqueId> uids, SearchQuery query, IList<OrderBy> orderBy, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously sort messages matching the specified query.
		/// </summary>
		/// <remarks>
		/// Asynchronously searches the folder for messages matching the specified query,
		/// returning the search results in the specified sort order.
		/// </remarks>
		/// <returns>The search results.</returns>
		/// <param name="options">The search options.</param>
		/// <param name="uids">The subset of UIDs</param>
		/// <param name="query">The search query.</param>
		/// <param name="orderBy">The sort order.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<SearchResults> SortAsync (SearchOptions options, IList<UniqueId> uids, SearchQuery query, IList<OrderBy> orderBy, CancellationToken cancellationToken = default);

		/// <summary>
		/// Thread the messages in the folder that match the search query using the specified threading algorithm.
		/// </summary>
		/// <remarks>
		/// The <see cref="MessageThread.UniqueId"/> can be used with methods such as
		/// <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken,ITransferProgress)"/>.
		/// </remarks>
		/// <returns>An array of message threads.</returns>
		/// <param name="algorithm">The threading algorithm to use.</param>
		/// <param name="query">The search query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IList<MessageThread> Thread (ThreadingAlgorithm algorithm, SearchQuery query, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously thread the messages in the folder that match the search query using the specified threading algorithm.
		/// </summary>
		/// <remarks>
		/// The <see cref="MessageThread.UniqueId"/> can be used with methods such as
		/// <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken,ITransferProgress)"/>.
		/// </remarks>
		/// <returns>An array of message threads.</returns>
		/// <param name="algorithm">The threading algorithm to use.</param>
		/// <param name="query">The search query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IList<MessageThread>> ThreadAsync (ThreadingAlgorithm algorithm, SearchQuery query, CancellationToken cancellationToken = default);

		/// <summary>
		/// Thread the messages in the folder that match the search query using the specified threading algorithm.
		/// </summary>
		/// <remarks>
		/// The <see cref="MessageThread.UniqueId"/> can be used with methods such as
		/// <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken,ITransferProgress)"/>.
		/// </remarks>
		/// <returns>An array of message threads.</returns>
		/// <param name="uids">The subset of UIDs</param>
		/// <param name="algorithm">The threading algorithm to use.</param>
		/// <param name="query">The search query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		IList<MessageThread> Thread (IList<UniqueId> uids, ThreadingAlgorithm algorithm, SearchQuery query, CancellationToken cancellationToken = default);

		/// <summary>
		/// Asynchronously thread the messages in the folder that match the search query using the specified threading algorithm.
		/// </summary>
		/// <remarks>
		/// The <see cref="MessageThread.UniqueId"/> can be used with methods such as
		/// <see cref="IMailFolder.GetMessage(UniqueId,CancellationToken,ITransferProgress)"/>.
		/// </remarks>
		/// <returns>An array of message threads.</returns>
		/// <param name="uids">The subset of UIDs</param>
		/// <param name="algorithm">The threading algorithm to use.</param>
		/// <param name="query">The search query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		Task<IList<MessageThread>> ThreadAsync (IList<UniqueId> uids, ThreadingAlgorithm algorithm, SearchQuery query, CancellationToken cancellationToken = default);

		/// <summary>
		/// Occurs when the folder is opened.
		/// </summary>
		/// <remarks>
		/// Emitted when the folder is opened.
		/// </remarks>
		event EventHandler<EventArgs> Opened;

		/// <summary>
		/// Occurs when the folder is closed.
		/// </summary>
		/// <remarks>
		/// Emitted when the folder is closed.
		/// </remarks>
		event EventHandler<EventArgs> Closed;

		/// <summary>
		/// Occurs when the folder is deleted.
		/// </summary>
		/// <remarks>
		/// Emitted when the folder is deleted.
		/// </remarks>
		event EventHandler<EventArgs> Deleted;

		/// <summary>
		/// Occurs when the folder is renamed.
		/// </summary>
		/// <remarks>
		/// Emitted when the folder is renamed.
		/// </remarks>
		event EventHandler<FolderRenamedEventArgs> Renamed;

		/// <summary>
		/// Occurs when the folder is subscribed.
		/// </summary>
		/// <remarks>
		/// Emitted when the folder is subscribed.
		/// </remarks>
		event EventHandler<EventArgs> Subscribed;

		/// <summary>
		/// Occurs when the folder is unsubscribed.
		/// </summary>
		/// <remarks>
		/// Emitted when the folder is unsubscribed.
		/// </remarks>
		event EventHandler<EventArgs> Unsubscribed;

		/// <summary>
		/// Occurs when a message is expunged from the folder.
		/// </summary>
		/// <remarks>
		/// Emitted when a message is expunged from the folder.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\ImapIdleExample.cs"/>
		/// </example>
		event EventHandler<MessageEventArgs> MessageExpunged;

		/// <summary>
		/// Occurs when messages vanish from the folder.
		/// </summary>
		/// <remarks>
		/// Emitted when a messages vanish from the folder.
		/// </remarks>
		event EventHandler<MessagesVanishedEventArgs> MessagesVanished;

		/// <summary>
		/// Occurs when flags changed on a message.
		/// </summary>
		/// <remarks>
		/// Emitted when flags changed on a message.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\ImapIdleExample.cs"/>
		/// </example>
		event EventHandler<MessageFlagsChangedEventArgs> MessageFlagsChanged;

		/// <summary>
		/// Occurs when labels changed on a message.
		/// </summary>
		/// <remarks>
		/// Emitted when labels changed on a message.
		/// </remarks>
		event EventHandler<MessageLabelsChangedEventArgs> MessageLabelsChanged;

		/// <summary>
		/// Occurs when annotations changed on a message.
		/// </summary>
		/// <remarks>
		/// Emitted when annotations changed on a message.
		/// </remarks>
		event EventHandler<AnnotationsChangedEventArgs> AnnotationsChanged;

		/// <summary>
		/// Occurs when a message summary is fetched from the folder.
		/// </summary>
		/// <remarks>
		/// <para>Emitted when a message summary is fetched from the folder.</para>
		/// <para>When multiple message summaries are being fetched from a remote folder,
		/// it is possible that the connection will drop or some other exception will
		/// occur, causing the Fetch method to fail, requiring the client to request the
		/// same set of message summaries again after it reconnects. This is obviously
		/// inefficient. To alleviate this potential problem, this event will be emitted
		/// as soon as the <see cref="IMailFolder"/> successfully retrieves the complete
		/// <see cref="IMessageSummary"/> for each requested message.</para>
		/// <note type="note">The <a href="Overload_MailKit_IMailFolder_Fetch.htm">Fetch</a>
		/// methods will return a list of all message summaries that any information was
		/// retrieved for, regardless of whether or not all of the requested items were fetched,
		/// therefore there may be a discrepency between the number of times this event is
		/// emitetd and the number of summary items returned from the Fetch method.</note>
		/// </remarks>
		event EventHandler<MessageSummaryFetchedEventArgs> MessageSummaryFetched;

		/// <summary>
		/// Occurs when metadata changes.
		/// </summary>
		/// <remarks>
		/// The <see cref="MetadataChanged"/> event is emitted when metadata changes.
		/// </remarks>
		event EventHandler<MetadataChangedEventArgs> MetadataChanged;

		/// <summary>
		/// Occurs when the mod-sequence changed on a message.
		/// </summary>
		/// <remarks>
		/// Emitted when the mod-sequence changed on a message.
		/// </remarks>
		event EventHandler<ModSeqChangedEventArgs> ModSeqChanged;

		/// <summary>
		/// Occurs when the highest mod-sequence changes.
		/// </summary>
		/// <remarks>
		/// The <see cref="HighestModSeqChanged"/> event is emitted whenever the <see cref="HighestModSeq"/> value changes.
		/// </remarks>
		event EventHandler<EventArgs> HighestModSeqChanged;

		/// <summary>
		/// Occurs when the next UID changes.
		/// </summary>
		/// <remarks>
		/// Emitted when the <see cref="UidNext"/> property changes.
		/// </remarks>
		event EventHandler<EventArgs> UidNextChanged;

		/// <summary>
		/// Occurs when the UID validity changes.
		/// </summary>
		/// <remarks>
		/// Emitted when the <see cref="UidValidity"/> property changes.
		/// </remarks>
		event EventHandler<EventArgs> UidValidityChanged;

		/// <summary>
		/// Occurs when the ID changes.
		/// </summary>
		/// <remarks>
		/// Emitted when the <see cref="Id"/> property changes.
		/// </remarks>
		event EventHandler<EventArgs> IdChanged;

		/// <summary>
		/// Occurs when the size of the folder changes.
		/// </summary>
		/// <remarks>
		/// Emitted when the <see cref="Size"/> property changes.
		/// </remarks>
		event EventHandler<EventArgs> SizeChanged;

		/// <summary>
		/// Occurs when the message count changes.
		/// </summary>
		/// <remarks>
		/// Emitted when the <see cref="Count"/> property changes.
		/// </remarks>
		/// <example>
		/// <code language="c#" source="Examples\ImapIdleExample.cs"/>
		/// </example>
		event EventHandler<EventArgs> CountChanged;

		/// <summary>
		/// Occurs when the recent message count changes.
		/// </summary>
		/// <remarks>
		/// Emitted when the <see cref="Recent"/> property changes.
		/// </remarks>
		event EventHandler<EventArgs> RecentChanged;

		/// <summary>
		/// Occurs when the message unread count changes.
		/// </summary>
		/// <remarks>
		/// Emitted when the <see cref="Unread"/> property changes.
		/// </remarks>
		event EventHandler<EventArgs> UnreadChanged;
	}
}
