# AKSupport

![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/StevenJDH/AKSupport?include_prereleases)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/15ee917d85e94a3a95f213f923a0b7ba)](https://www.codacy.com/gh/StevenJDH/AKSupport/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=StevenJDH/AKSupport&amp;utm_campaign=Badge_Grade)
![Maintenance](https://img.shields.io/maintenance/yes/2021)
![GitHub](https://img.shields.io/github/license/StevenJDH/AKSupport)

AKSupport is an automation tool that regularly checks for the current supported status of an AKS cluster to alert those interested that an upgrade is needed to maintain Microsoft support. In other words, rather than always manually checking different AKS clusters across multiple environments to keep up with the [Kubernetes version support policy](https://docs.microsoft.com/en-us/azure/aks/supported-kubernetes-versions#kubernetes-version-support-policy), AKSupport will manage all this for you. This also covers situations where a supported version is pulled early by Microsoft due to security concerns or other critical issues, which they reserve the right to do. In the end, the goal is to free people up for other value-add tasks by keeping things simple for the many uses cases that can benefit from such a tool like general maintenance, Infrastructure as Code (IaC) deployments, etc.

[![Buy me a coffee](https://img.shields.io/static/v1?label=Buy%20me%20a&message=coffee&color=important&style=flat&logo=buy-me-a-coffee&logoColor=white)](https://www.buymeacoffee.com/stevenjdh)

Releases: [https://github.com/StevenJDH/AKSupport/releases](https://github.com/StevenJDH/AKSupport/releases)

## Features
* Out of support checking.
* Near end of support checking.
* Works with all regions.
* Provides feedback for Azure triggers and action groups.
* **Coming Soon:** Support for sending alert mails directly via Microsoft Graph API.  

## Prerequisites
* An Azure Kubernetes Service (AKS) cluster (Use CronJob `apiVersion: batch/v1` for v1.21+)
* Last version of [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli).
* An Azure [Service Principal](https://docs.microsoft.com/en-us/cli/azure/ad/sp?view=azure-cli-latest#az_ad_sp_create_for_rbac) for API access.

## Usage
The following are the steps needed to set up AKSupport correctly along with the needed API permissions:
1. In a console window, type `az ad sp create-for-rbac -n "AKSupport"` to create a [Service Principal](https://docs.microsoft.com/en-us/cli/azure/ad/sp?view=azure-cli-latest#az_ad_sp_create_for_rbac) in Azure. Currently, this assigns `Contributor` rights to the SP, but in the future, this will change. Use `--skip-assignment` to skip the role assignment if a more specific role is needed, and then follow the [Steps to assign an Azure role](https://docs.microsoft.com/en-us/azure/role-based-access-control/role-assignments-steps) article. The output from the command should look like the following:

   ```json
   {
     "appId": "...",
     "displayName": "AKSupport",
     "name": "http://AKSupport",
     "password": "...",
     "tenant": "..."
   }
   ```

2. Create a Kubernetes Secret in AKS for the SP `password` by typing:

   ```bash
   kubectl create secret generic aksupport-secret --from-literal=AZURE_APP_PASSWORD=<password>
   ```

3. Create a Kubernetes ConfigMap in AKS for the SP appId and tenant fields along with the AKS subscription id and region by typing:

   ```bash
   kubectl create configmap aksupport-config -from-literal=AZURE_SUBSCRIPTION_ID=<subscriptionId> \
       --from-literal=AZURE_APP_TENANT=<tenant> \
       --from-literal=AZURE_APP_ID=<appId> \
       --from-literal=AZURE_AKS_REGION=<region>
   ```

4. Create a Kubernetes CronJob in AKS for AKSupport by using the following command and provided definition:

   ```bash
   kubectl create -f https://raw.githubusercontent.com/StevenJDH/AKSupport/main/YAML/aksupport-cronjob.yaml
   ```

5. Assuming everything was set up correctly, AKSupport will run at the default configured time via the CronJob at 8:00am every morning. To confirm the creation of the AKSupport CronJob, type:

   ```bash
   kubectl get cronjobs
   ```
In addition to the imperative commands used to create the Secret and the ConfigMap above, YAML versions have also been provided if a declarative approach is preferred. Just remember to [Base64 encode](https://www.base64encode.net/) the value for the Secret if using YAML.

## Exit codes
Every time that AKSupport finishes its checks, it will return one of three exit codes. To view the last exit code, type:

```bash
kubectl get pod <aksupport-cronjob-00-00> --output="jsonpath={.status.containerStatuses[].state.terminated.exitCode}"
```

Exit codes can be useful for Triggers configured in Azure or other monitoring tools to perform additional tasks like triggering an Action Group.

|Exit Code  |Meaning                                                                       
|:---------:|:--------------
|0          |The version of Kubernetes in AKS is still supported.
|1          |The version of Kubernetes in AKS is not supported.
|2          |The version of Kubernetes in AKS is about to lose support.

## Testing
To test AKSupport with a specific version to see if it works correctly, edit the [CronJob definition](https://raw.githubusercontent.com/StevenJDH/AKSupport/main/YAML/aksupport-cronjob.yaml) by uncommenting the `args` section like below and by adding the desired version in quotes.

```yaml
...
containers:
- name: aksupport
  image: public.ecr.aws/stevenjdh/aksupport
  # Uncomment below for testing a specific version.
  args:
  - "1.17.0"
...
```

Also, change the `schedule` field to `* * * * *` to run AKSupport every minute while testing or to any other [cron expression](https://crontab.guru/) as needed. Use `az aks get-versions --location northeurope --output table` to see what versions are currently supported for a particular region and their upgrade paths as a reference.

## Additional information
When AKSupport finishes checking the environment, the underlying pod will either have a `Completed` status or an `Error` status by design. Only the last state will be visible at any given time. To view the logs for the last run, type:

```bash
kubectl logs <aksupport-cronjob-00-00>
```

In the logs, there will be additional information for the result of the check.

## Disclaimer
AKSupport is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

## Contributing
Thanks for your interest in contributing! There are many ways to contribute to this project. Get started [here](https://github.com/StevenJDH/.github/blob/main/docs/CONTRIBUTING.md).

## Do you have any questions?
Many commonly asked questions are answered in the FAQ:
[https://github.com/StevenJDH/AKSupport/wiki/FAQ](https://github.com/StevenJDH/AKSupport/wiki/FAQ)

## Want to show your support?

|Method       | Address                                                                                                    |
|------------:|:-----------------------------------------------------------------------------------------------------------|
|PayPal:      | [https://www.paypal.me/stevenjdh](https://www.paypal.me/stevenjdh "Steven's Paypal Page")                  |
|Bitcoin:     | 3GyeQvN6imXEHVcdwrZwKHLZNGdnXeDfw2                                                                         |
|Litecoin:    | MAJtR4ccdyUQtiiBpg9PwF2AZ6Xbk5ioLm                                                                         |
|Ethereum:    | 0xa62b53c1d49f9C481e20E5675fbffDab2Fcda82E                                                                 |
|Dash:        | Xw5bDL93fFNHe9FAGHV4hjoGfDpfwsqAAj                                                                         |
|Zcash:       | t1a2Kr3jFv8WksgPBcMZFwiYM8Hn5QCMAs5                                                                        |
|PIVX:        | DQq2qeny1TveZDcZFWwQVGdKchFGtzeieU                                                                         |
|Ripple:      | rLHzPsX6oXkzU2qL12kHCH8G8cnZv1rBJh<br />Destination Tag: 2357564055                                        |
|Monero:      | 4GdoN7NCTi8a5gZug7PrwZNKjvHFmKeV11L6pNJPgj5QNEHsN6eeX3D<br />&#8618;aAQFwZ1ufD4LYCZKArktt113W7QjWvQ7CWDXrwM8yCGgEdhV3Wt|


// Steven Jenkins De Haro ("StevenJDH" on GitHub)
