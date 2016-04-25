/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

// MySQL DB access module, for use by plugins and others
// For the module that implements interactive DB functionality see mod_db

#ifndef _MOD_CRUD_COLLECTION_ADD_H_
#define _MOD_CRUD_COLLECTION_ADD_H_

#include "collection_crud_definition.h"

namespace mysh
{
  namespace mysqlx
  {
    class Collection;

    /**
    * Handler for document addition on a Collection.
    *
    * This object provides the necessary functions to allow adding documents into a collection.
    *
    * This object should only be created by calling any of the add functions on the collection object where the documents will be added.
    *
    * \sa Collection
    */
    class CollectionAdd : public Collection_crud_definition, public boost::enable_shared_from_this<CollectionAdd>
    {
    public:
      CollectionAdd(boost::shared_ptr<Collection> owner);

      virtual std::string class_name() const { return "CollectionAdd"; }

      shcore::Value add(const shcore::Argument_list &args);
      virtual shcore::Value execute(const shcore::Argument_list &args);

#ifdef DOXYGEN
      CollectionAdd add(Document document);
      CollectionAdd add(List documents);
      Result execute();
#endif

    private:
      std::string get_new_uuid();

      std::string _last_document_id;

      std::unique_ptr< ::mysqlx::AddStatement> _add_statement;
    };
  }
}

#endif
