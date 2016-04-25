/*
   Copyright (c) 2015, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

// MySQL DB access module, for use by plugins and others
// For the module that implements interactive DB functionality see mod_db

#ifndef _MOD_CRUD_COLLECTION_REMOVE_H_
#define _MOD_CRUD_COLLECTION_REMOVE_H_

#include "collection_crud_definition.h"

namespace mysh
{
  namespace mysqlx
  {
    class Collection;

    /**
    * Handler for document removal from a Collection.
    *
    * This object provides the necessary functions to allow removing documents from a collection.
    *
    * This object should only be created by calling the remove function on the collection object from which the documents will be removed.
    *
    * \sa Collection
    */
    class CollectionRemove : public Collection_crud_definition, public boost::enable_shared_from_this<CollectionRemove>
    {
    public:
      CollectionRemove(boost::shared_ptr<Collection> owner);
    public:
      virtual std::string class_name() const { return "CollectionRemove"; }
      static boost::shared_ptr<shcore::Object_bridge> create(const shcore::Argument_list &args);
      shcore::Value remove(const shcore::Argument_list &args);
      shcore::Value sort(const shcore::Argument_list &args);
      shcore::Value limit(const shcore::Argument_list &args);
      shcore::Value bind(const shcore::Argument_list &args);

      virtual shcore::Value execute(const shcore::Argument_list &args);
#ifdef DOXYGEN
      CollectionRemove remove(String searchCondition);
      CollectionRemove sort(List sortExprStr);
      CollectionRemove limit(Integer numberOfRows);
      CollectionFind bind(String name, Value value);
      Result execute(ExecuteOptions opt);
#endif
    private:
      std::unique_ptr< ::mysqlx::RemoveStatement> _remove_statement;
    };
  };
};

#endif
