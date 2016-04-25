/*
   Copyright (c) 2016, Oracle and/or its affiliates. All rights reserved.














   The lines above are intentionally left blank
*/

#ifndef _SHELLNOTIFICATIONS_H_
#define _SHELLNOTIFICATIONS_H_

#include "shellcore/common.h"
#include "shellcore/types.h"

namespace shcore
{
  class SHCORE_PUBLIC NotificationObserver
  {
  public:
    virtual void handle_notification(const std::string &name, shcore::Object_bridge_ref sender, shcore::Value::Map_type_ref data) = 0;
    void observe_notification(const std::string& notification);
    void ignore_notification(const std::string& notification);
    virtual ~NotificationObserver();

  private:
    std::list<std::string> _notifications;
  };

  typedef std::list<NotificationObserver*> ObserverList;

  class SHCORE_PUBLIC ShellNotifications
  {
  private:
    ShellNotifications(){}
    std::map<std::string, ObserverList*> _observers;

    static ShellNotifications* _instance;

  public:
    static ShellNotifications *get();
    virtual ~ShellNotifications();

    bool add_observer(NotificationObserver *observer, const std::string &notification);
    bool remove_observer(NotificationObserver *observer, const std::string &notification);
    void notify(const std::string &name, shcore::Object_bridge_ref sender, shcore::Value::Map_type_ref data);
    void notify(const std::string &name, shcore::Object_bridge_ref sender);
  };
};

#endif // _SHELLCORE_H_
